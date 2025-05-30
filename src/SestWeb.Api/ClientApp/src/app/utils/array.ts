export class ArrayUtils {
  /**
   * Performs a binary search looking for a given number in a given array.
   *
   * @param arr the array in which to search the value.
   * @param val the value to be searched.
   */
  static binarySearch(arr: Array<number>, val: number) {
    /**
     * Lines below are not thoroughly commented because it's a standard
     * binary search.
     */
    let begin = 0, end = arr.length - 1;

    while (begin < end) {
      const pivot = Math.floor((begin + end) / 2);

      if (arr[pivot] === val) {
        return pivot;
      } else if (val < arr[pivot]) {
        end = pivot - 1;
      } else {
        begin = pivot + 1;
      }
    }

    return begin;
  }

  /**
   * Compares two arrays and make sure they have the same elements in the same
   * order.
   *
   * @param arr1 first array to check.
   * @param arr2 second array to compare.
   */
  static equals(arr1: Array<any>, arr2: Array<any>) {
    if (!(arr1 instanceof Array && arr2 instanceof Array)) {
      return false;
    }

    if (arr1.length !== arr2.length) {
      return false;
    }

    for (let i = 0, l = arr1.length; i < l; i++) {
      if (arr1[i] instanceof Array && arr2[i] instanceof Array) {
        if (!ArrayUtils.equals(arr1[i], arr2[i])) {
          return false;
        }
      } else if (arr1[i] !== arr2[i]) {
        return false;
      }
    }
    return true;
  }

  /**
   * Copy an array and remove duplicates
   *
   * @param arr array to copy.
   */
  static removeDups(arr: Array<any>) {
    if (arr === null) {
      return [];
    }
    const unique = [];
    for (let i = 0; i < arr.length; i++) {
      if (unique.indexOf(arr[i]) === -1) {
        unique.push(arr[i]);
      }
    }
    return unique;
  }

  /**
   * Removes the first occurrence of the element in the array.
   *
   * @param arr array.
   * @param toRemove element to remove.
   */
  static removeFirst<T>(array: T[], toRemove: T): void {
    const index = array.indexOf(toRemove);
    if (index !== -1) {
      array.splice(index, 1);
    }
  }

  /**
   * Merge some arrays without duplicates.
   *
   * @param arrays arrays to merge.
   */
  static mergeWithoutDups(...arrays) {
    arrays = arrays.flat();
    return this.removeDups(arrays);
  }

  static stringLitArray = <L extends string>(arr: L[]) => arr;
}
