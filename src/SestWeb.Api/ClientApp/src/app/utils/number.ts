export class NumberUtils {

    /**
     * Determines whether or not a given input is a valid Javascript number.
     *
     * @param value the value to test.
     */
    static isNumber(value: string | number) {
        if (value === null || value === undefined || value === '') {
            return false;
        }

        if (typeof (value) === 'number') {
            return true;
        }

        return !Number.isNaN(Number(value));
    }

    static changeDecimalPlaces(value: number, decimalPlaces: number) {
        return +value.toFixed(decimalPlaces);
    }
}