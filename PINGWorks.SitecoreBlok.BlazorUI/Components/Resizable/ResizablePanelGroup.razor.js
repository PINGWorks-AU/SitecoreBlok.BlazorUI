// Resizable panel drag engine.
// init() attaches pointer + keyboard handlers to every resizable-handle inside a group.
// dispose() removes them. Each call to init() is keyed by the group DOM element.

const groups = new Map();

function getPanelsForHandle(handle) {
	const parent = handle.parentElement;
	if (!parent) return null;

	const children = Array.from(parent.children);
	const idx = children.indexOf(handle);
	if (idx < 0) return null;

	let prev = null;
	for (let i = idx - 1; i >= 0; i--) {
		if (children[i].dataset.slot === 'resizable-panel') { prev = children[i]; break; }
	}

	let next = null;
	for (let i = idx + 1; i < children.length; i++) {
		if (children[i].dataset.slot === 'resizable-panel') { next = children[i]; break; }
	}

	return prev && next ? [prev, next] : null;
}

function clamp(value, min, max) {
	return Math.max(min, Math.min(max, value));
}

function getConstraints(panel, totalPx) {
	const minPct = parseFloat(panel.dataset.panelMinSize ?? '0');
	const maxPct = parseFloat(panel.dataset.panelMaxSize ?? '100');
	return { min: minPct / 100 * totalPx, max: maxPct / 100 * totalPx };
}

// Percentage basis (not px) so panels keep tracking the group width when the
// window/container resizes — a fixed px basis freezes the split at drag-time width.
function setFlexPct(panel, px, totalPx) {
	const pct = totalPx > 0 ? (px / totalPx) * 100 : 0;
	panel.style.flex = `0 0 ${pct}%`;
}

function applyResize(panels, newPrevPx, totalPx) {
	const pc = getConstraints(panels[0], totalPx);
	const nc = getConstraints(panels[1], totalPx);

	// Clamp prev, derive next, clamp next, re-derive prev to maintain total.
	let prevPx = clamp(newPrevPx, pc.min, pc.max);
	let nextPx = clamp(totalPx - prevPx, nc.min, nc.max);
	prevPx = clamp(totalPx - nextPx, pc.min, pc.max);

	setFlexPct(panels[0], prevPx, totalPx);
	setFlexPct(panels[1], nextPx, totalPx);
}

function initHandle(handle, cleanups) {
	let dragging = false;
	let startClientPos = 0;
	let startPrevPx = 0;
	let startTotalPx = 0;
	let activePanels = null;

	const isVertical = () => handle.dataset.panelGroupDirection === 'vertical';

	function onPointerDown(e) {
		if (e.button !== 0) return;

		const panels = getPanelsForHandle(handle);
		if (!panels) return;

		const isVert = isVertical();
		const prevRect = panels[0].getBoundingClientRect();
		const nextRect = panels[1].getBoundingClientRect();

		startPrevPx = isVert ? prevRect.height : prevRect.width;
		const startNextPx = isVert ? nextRect.height : nextRect.width;
		startTotalPx = startPrevPx + startNextPx;
		startClientPos = isVert ? e.clientY : e.clientX;

		// Normalize both panels to explicit flex-basis before dragging.
		setFlexPct(panels[0], startPrevPx, startTotalPx);
		setFlexPct(panels[1], startNextPx, startTotalPx);

		activePanels = panels;
		dragging = true;
		handle.setPointerCapture(e.pointerId);
		e.preventDefault();
	}

	function onPointerMove(e) {
		if (!dragging || !activePanels) return;
		const delta = (isVertical() ? e.clientY : e.clientX) - startClientPos;
		applyResize(activePanels, startPrevPx + delta, startTotalPx);
	}

	function onPointerUp() {
		dragging = false;
		activePanels = null;
	}

	function onKeyDown(e) {
		const panels = getPanelsForHandle(handle);
		if (!panels) return;

		const isVert = isVertical();
		// Arrow keys resize by 10px; Shift+Arrow by 50px.
		const step = e.shiftKey ? 50 : 10;

		let delta = 0;
		let goHome = false;
		let goEnd = false;

		if      (e.key === (isVert ? 'ArrowUp'   : 'ArrowLeft'))  delta = -step;
		else if (e.key === (isVert ? 'ArrowDown'  : 'ArrowRight')) delta = step;
		else if (e.key === 'Home') goHome = true;
		else if (e.key === 'End')  goEnd  = true;
		else return;

		e.preventDefault();

		const prevRect = panels[0].getBoundingClientRect();
		const nextRect = panels[1].getBoundingClientRect();
		const prevPx   = isVert ? prevRect.height : prevRect.width;
		const nextPx   = isVert ? nextRect.height : nextRect.width;
		const totalPx  = prevPx + nextPx;

		// Normalize to explicit sizes before adjusting.
		setFlexPct(panels[0], prevPx, totalPx);
		setFlexPct(panels[1], nextPx, totalPx);

		let newPrevPx;
		if      (goHome) newPrevPx = getConstraints(panels[0], totalPx).min;
		else if (goEnd)  newPrevPx = getConstraints(panels[0], totalPx).max;
		else             newPrevPx = prevPx + delta;

		applyResize(panels, newPrevPx, totalPx);
	}

	handle.addEventListener('pointerdown', onPointerDown);
	handle.addEventListener('pointermove', onPointerMove);
	handle.addEventListener('pointerup',   onPointerUp);
	handle.addEventListener('pointercancel', onPointerUp);
	handle.addEventListener('keydown',     onKeyDown);

	cleanups.push(() => {
		handle.removeEventListener('pointerdown', onPointerDown);
		handle.removeEventListener('pointermove', onPointerMove);
		handle.removeEventListener('pointerup',   onPointerUp);
		handle.removeEventListener('pointercancel', onPointerUp);
		handle.removeEventListener('keydown',     onKeyDown);
	});
}

export function init(panelGroupEl) {
	if (!panelGroupEl) return;

	const cleanups = [];
	panelGroupEl.querySelectorAll('[data-slot="resizable-handle"]').forEach(
		h => initHandle(h, cleanups)
	);

	groups.set(panelGroupEl, () => cleanups.forEach(fn => fn()));
}

export function dispose(panelGroupEl) {
	const cleanup = groups.get(panelGroupEl);
	if (cleanup) {
		cleanup();
		groups.delete(panelGroupEl);
	}
}
