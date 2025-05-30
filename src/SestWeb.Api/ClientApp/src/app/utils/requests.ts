/**
 * Defines an interface consisting of objects where every key-value pair
 * consists solely of strings (both key and value are strings).
 */
interface URIEncodableObject {
    [key: string]: string;
}

export class RequestUtils {
    /**
     * Receives an object and converts it into a URI (param=value&other=foo&...).
     *
     * @param obj the object to be converted into a URI.
     */
    static toURI(obj: URIEncodableObject) {
        // Create an alias to make lines shorter
        const encode = encodeURIComponent;

        /**
         * The keys of the original object are fetched; then, for each position
         * of the array containing the object's keys, that position will become a
         * concatenation of {key}={value}, URI encoded. In the end, every position of
         * the array is joined using an '&'.
         */
        return Object.keys(obj).map(
            key => encode(key) + '=' + encode(obj[key])
        ).join('&');
    }
}
