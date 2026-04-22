export class SitecoreUI {

	static getElementBounds = function (e) {

		try {
			return e.getBoundingClientRect();
		} catch {
			return {};
		}
	}

	static getWindowBounds = function () {

		try {
			return {
				windowWidth: window.innerWidth,
				windowHeight: window.innerHeight,
				scrollX: window.scrollX,
				scrollY: window.scrollY,
				docHeight: document.body.clientHeight,
				docWidth: document.body.clientWidth
			};
		} catch {
			return {};
		}
	};

	// Two-pass context menu positioning: call after the menu is rendered (visibility:hidden)
	// to measure its size, then flip axes if they would overflow the viewport.
	static calcContextMenuPosition = function (element, clickX, clickY) {

		try {
			const rect = element.getBoundingClientRect();
			const vpW = window.innerWidth;
			const vpH = window.innerHeight;
			let x = clickX;
			let y = clickY;
			if (x + rect.width > vpW) x = Math.max(0, x - rect.width);
			if (y + rect.height > vpH) y = Math.max(0, y - rect.height);
			return { X: x, Y: y };
		} catch {
			return { X: clickX, Y: clickY };
		}
	};

	// Two-pass submenu positioning: anchors to the right edge of the trigger by default,
	// flips to the left if it would overflow the viewport right edge.
	static calcSubmenuPosition = function (element, triggerLeft, triggerTop, triggerRight) {

		try {
			const rect = element.getBoundingClientRect();
			const vpW = window.innerWidth;
			const vpH = window.innerHeight;
			let x = triggerRight;
			let y = triggerTop;
			if (x + rect.width > vpW) x = Math.max(0, triggerLeft - rect.width);
			if (y + rect.height > vpH) y = Math.max(0, vpH - rect.height);
			return { X: x, Y: y };
		} catch {
			return { X: triggerRight, Y: triggerTop };
		}
	};

	// Two-pass dropdown positioning: mirrors Radix avoidCollisions behaviour.
	// side/align match the Blok prop names (lowercase). sideOffset defaults to 4 for Content, 0 for SubContent.
	static calcDropdownPosition = function (element, triggerLeft, triggerTop, triggerRight, triggerBottom, side, align, sideOffset) {

		try {
			const rect = element.getBoundingClientRect();
			const menuW = rect.width, menuH = rect.height;
			const triggerW = triggerRight - triggerLeft;
			const triggerH = triggerBottom - triggerTop;
			const vpW = window.innerWidth, vpH = window.innerHeight;
			let x, y;

			if (side === 'bottom' || side === 'top') {
				const yBelow = triggerBottom + sideOffset;
				const yAbove = triggerTop - menuH - sideOffset;
				let openBelow = side === 'bottom';
				if (openBelow && yBelow + menuH > vpH && yAbove >= 0) openBelow = false;
				if (!openBelow && yAbove < 0 && yBelow + menuH <= vpH) openBelow = true;
				y = openBelow ? yBelow : yAbove;

				if (align === 'start') x = triggerLeft;
				else if (align === 'center') x = triggerLeft + (triggerW - menuW) / 2;
				else x = triggerRight - menuW;

				if (x + menuW > vpW) x = vpW - menuW;
				if (x < 0) x = 0;
			} else {
				const xRight = triggerRight + sideOffset;
				const xLeft = triggerLeft - menuW - sideOffset;
				let openRight = side === 'right';
				if (openRight && xRight + menuW > vpW && xLeft >= 0) openRight = false;
				if (!openRight && xLeft < 0 && xRight + menuW <= vpW) openRight = true;
				x = openRight ? xRight : xLeft;

				if (align === 'start') y = triggerTop;
				else if (align === 'center') y = triggerTop + (triggerH - menuH) / 2;
				else y = triggerBottom - menuH;

				if (y + menuH > vpH) y = vpH - menuH;
				if (y < 0) y = 0;
			}

			return { X: x, Y: y };
		} catch {
			return { X: triggerLeft, Y: triggerBottom };
		}
	};

	static selectAll = function (element) {
		try { element.setSelectionRange(0, element.value.length); } catch { try { element.select(); } catch { } }
	};

	static focusAndSelectAll = function (element) {
		try {
			element.focus();
			SitecoreUI.selectAll(element);
		} catch { }
	};
}
