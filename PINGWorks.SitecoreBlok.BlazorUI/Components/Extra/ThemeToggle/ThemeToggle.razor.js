const STORAGE_KEY = "displayMode";

function applyMode( enableDarkMode ) {
	document.querySelectorAll("*[data-dark-mode-target]").forEach(
		function (e) {
			if (enableDarkMode)
				e.classList.add("dark");
			else
				e.classList.remove("dark");
		}
	);

	// Keep the <html> root in sync with the wrapper. The first-paint head
	// script in App.razor adds `.dark` to <html> on initial load; on toggle
	// we mirror that here so light-mode toggle removes the html-level class
	// too. Without this, toggling to light leaves <html class="dark"> behind
	// and dark-mode CSS (variables and `dark:` variants) keeps applying via
	// the html ancestor even after the wrapper class is removed.
	if (enableDarkMode)
		document.documentElement.classList.add("dark");
	else
		document.documentElement.classList.remove("dark");

	// `color-scheme` on the <html> element. Native form controls (select
	// dropdowns, scrollbars, date/time pickers, range thumbs) are rendered
	// by the browser as page chrome and key off the document root's
	// color-scheme — NOT off any deeper element. Without this line, the
	// Calendar's month/year <select> popups stay light-themed even when
	// our app is in dark mode.
	document.documentElement.style.colorScheme = enableDarkMode ? "dark" : "light";
}

function applyStoredDisplayMode() {
	const stored = getStoredDisplayMode();
	if ( stored === null ) return;
	applyMode( stored );
}

export function setDisplayMode( enableDarkMode ) {
	applyMode( enableDarkMode );

	try {
		localStorage.setItem( STORAGE_KEY, enableDarkMode ? "dark" : "light" );
	} catch (e) {
		// localStorage may be unavailable (private browsing, restricted contexts) — ignore.
	}
}

export function getStoredDisplayMode() {
	try {
		const v = localStorage.getItem( STORAGE_KEY );
		if ( v === "dark" ) return true;
		if ( v === "light" ) return false;
	} catch (e) {
		// localStorage unavailable — fall through to null.
	}
	return null;
}

// Blazor Enhanced Navigation replaces page content via server-side diff/patch.
// Without this listener, the `dark` class on the data-dark-mode-target element
// gets stripped on every enhanced navigation (because it's absent from the
// server-rendered HTML the diff compares against). Re-apply from localStorage
// on each enhanced load so dark mode survives client-side route changes.
if ( typeof Blazor !== "undefined" && typeof Blazor.addEventListener === "function" ) {
	Blazor.addEventListener( "enhancedload", applyStoredDisplayMode );
}
