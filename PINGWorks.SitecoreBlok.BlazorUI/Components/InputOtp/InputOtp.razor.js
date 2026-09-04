// Keeps the hidden input's raw text identical to the filtered value.
//
// When `Pattern` rejects a character, the browser has already placed it in the input. Blazor
// will not rewrite the DOM value in that case, because from its point of view the bound value
// never changed - so the raw text drifts ahead of the value. That matters because the caret
// position and any length limit are measured against the raw text.
export function syncValue( element, value ) {
	if ( element && element.value !== value ) {
		element.value = value;
		// Keep the caret at the end, otherwise it jumps to position 0 after the rewrite.
		const end = value.length;
		element.setSelectionRange( end, end );
	}
}
