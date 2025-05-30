/**
 * Check if the value is undefined, null or ''.
 *
 * @param value the value to test.
 */
export function UNSET(value) {
    return value === undefined || value === null || value === '';
}