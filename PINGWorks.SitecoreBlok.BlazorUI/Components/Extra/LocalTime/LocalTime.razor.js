// Component-isolated module for <LocalTime>. Loaded once per component instance
// via JS module import (browser-cached after the first request) and disposed
// with the component. No globals, no document-wide observers — the component
// hands its own <time> element to format() and we rewrite that one node.

// Longer alternatives must precede shorter ones (yyyy before yy, hh before h)
// so the alternation prefers the longer match at each position.
const FORMAT_TOKENS = /yyyy|yy|MM|dd|HH|hh|h|mm|ss|tt|K/g;

export function format( el, iso, pattern ) {
	if ( !el || !iso || !pattern )
		return;

	const d = new Date( iso );
	if ( Number.isNaN( d.getTime() ) )
		return;

	const pad = n => String( n ).padStart( 2, '0' );
	const hours24 = d.getHours();
	const hours12 = hours24 % 12 || 12;

	// JS getTimezoneOffset() returns minutes WEST of UTC (positive for west).
	// .NET's K specifier renders "+HH:mm" / "-HH:mm" — east of UTC is positive,
	// so flip the sign before formatting.
	const tzMinutes = -d.getTimezoneOffset();
	const tzSign = tzMinutes >= 0 ? '+' : '-';
	const tzAbs = Math.abs( tzMinutes );

	const map = {
		'yyyy': d.getFullYear(),
		'yy':   pad( d.getFullYear() % 100 ),
		'MM':   pad( d.getMonth() + 1 ),
		'dd':   pad( d.getDate() ),
		'HH':   pad( hours24 ),
		'hh':   pad( hours12 ),
		'h':    String( hours12 ),
		'mm':   pad( d.getMinutes() ),
		'ss':   pad( d.getSeconds() ),
		'tt':   hours24 < 12 ? 'AM' : 'PM',
		'K':    `${tzSign}${pad( Math.floor( tzAbs / 60 ) )}:${pad( tzAbs % 60 )}`
	};

	el.textContent = pattern.replace( FORMAT_TOKENS, t => map[ t ] );
}
