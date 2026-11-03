// Menubar keyboard navigation and content placement. Open state lives in Blazor; keys change it by clicking a trigger.

import { SitecoreUI } from '../../js/sitecoreUI.js';

const handlers = new WeakMap();

const TRIGGER = '[data-slot=menubar-trigger]';
const CONTENT = '[data-slot=menubar-content]';
const ITEM = ':is([role=menuitem], [role=menuitemcheckbox], [role=menuitemradio]):not([data-disabled])';

function triggersOf(root) {
	return Array.from(root.querySelectorAll(TRIGGER)).filter(t => t.closest('[data-slot=menubar]') === root);
}

function triggerFor(content) {
	return document.getElementById(content.getAttribute('aria-labelledby'));
}

function itemsOf(content) {
	return Array.from(content.querySelectorAll(ITEM)).filter(i => i.closest(CONTENT) === content);
}

function isOpen(trigger) {
	return trigger.dataset.state === 'open';
}

function wrap(list, index) {
	return list[(index + list.length) % list.length];
}

function moveToTrigger(root, from, step) {
	const triggers = triggersOf(root);
	const next = wrap(triggers, triggers.indexOf(from) + step);
	const anyOpen = triggers.some(isOpen);

	next.focus();

	if (anyOpen && !isOpen(next)) {
		next.click();
	}
}

function onTriggerKey(root, trigger, event) {
	switch (event.key) {
		case 'ArrowDown':
		case 'ArrowUp': {
			event.preventDefault();
			const content = document.getElementById(trigger.getAttribute('aria-controls'));

			if (content) {
				const items = itemsOf(content);
				items[event.key === 'ArrowDown' ? 0 : items.length - 1]?.focus();
			} else {
				trigger.click();
			}
			break;
		}
		case 'ArrowRight':
		case 'ArrowLeft':
			event.preventDefault();
			moveToTrigger(root, trigger, event.key === 'ArrowRight' ? 1 : -1);
			break;
	}
}

function onItemKey(root, content, item, event) {
	const trigger = triggerFor(content);
	const items = itemsOf(content);
	const index = items.indexOf(item);

	switch (event.key) {
		case 'ArrowDown':
			event.preventDefault();
			wrap(items, index + 1)?.focus();
			break;
		case 'ArrowUp':
			event.preventDefault();
			wrap(items, index - 1)?.focus();
			break;
		case 'Home':
			event.preventDefault();
			items[0]?.focus();
			break;
		case 'End':
			event.preventDefault();
			items[items.length - 1]?.focus();
			break;
		case 'Enter':
		case ' ':
			event.preventDefault();
			item.click();
			trigger?.focus();
			break;
		case 'ArrowRight':
		case 'ArrowLeft':
			if (trigger) {
				event.preventDefault();
				moveToTrigger(root, trigger, event.key === 'ArrowRight' ? 1 : -1);
			}
			break;
		case 'Tab':
			trigger?.click();
			break;
	}
}

export function attach(root) {
	detach(root);

	const handler = (event) => {
		const content = event.target.closest(CONTENT);

		if (content) {
			const item = event.target.closest(ITEM);

			if (item) {
				onItemKey(root, content, item, event);
			}

			return;
		}

		const trigger = event.target.closest(TRIGGER);

		if (trigger) {
			onTriggerKey(root, trigger, event);
		}
	};

	const escape = (event) => {
		const open = event.key === 'Escape' && triggersOf(root).find(isOpen);

		if (open) {
			event.preventDefault();
			open.focus();
			open.click();
		}
	};

	root.addEventListener('keydown', handler);
	document.addEventListener('keydown', escape);
	handlers.set(root, { handler, escape });
}

export function detach(root) {
	const attached = handlers.get(root);

	if (attached) {
		root.removeEventListener('keydown', attached.handler);
		document.removeEventListener('keydown', attached.escape);
		handlers.delete(root);
	}
}

// Positions the content under its trigger and reveals it in one step, so opening costs no extra round trip.
export function place(content, trigger, side, align, sideOffset, focusFirstItem) {
	if (!content || !trigger) {
		return;
	}

	const bounds = trigger.getBoundingClientRect();
	const position = SitecoreUI.calcDropdownPosition(content, bounds.left, bounds.top, bounds.right, bounds.bottom, side, align, sideOffset);

	content.style.left = `${position.X}px`;
	content.style.top = `${position.Y}px`;
	content.style.visibility = 'visible';

	if (focusFirstItem) {
		itemsOf(content)[0]?.focus();
		return;
	}

	const focused = document.activeElement;

	if (focused !== trigger && focused?.matches(TRIGGER) && focused.closest('[data-slot=menubar]') === trigger.closest('[data-slot=menubar]')) {
		trigger.focus();
	}
}
