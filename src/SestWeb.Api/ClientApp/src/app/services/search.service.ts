import { Subject } from 'rxjs';
import { Injectable } from '@angular/core';
import { FileService } from '../repositories/file.service';
import { ExplorerDataService } from './explorer-data.service';
import { OAuthTokenService } from './oauth.service';

@Injectable({
  providedIn: 'root'
})
export class ExplorerSearchService {
  /**
   * Whether or not the user can search.
   */
  searchIsReady: boolean = false;
  /**
   * A dict which will contain every single item in the system.
   */
  searchableItems = {
    units: [],
    oilfields: [],
    wells: [],
    files: []
  };
  /**
   * A dict which will hold all search results matching the user input.
   */
  searchResults = {
    units: [],
    oilfields: [],
    wells: [],
    files: []
  };
  /**
   * Whether or not the search mode should be disabled.
   */
  isSearching = false;
  /**
   * The total amount of items in the tree of a selected search result.
   * This number scales as the tree is recursively processed.
   */
  totalTreeItems = 0;
  /**
   * The amount of items in the tree that were already loaded.
   * This number scales as the tree is recursively loaded, and ideally
   * will match the totalTreeItems.
   */
  loadedTreeItems = 0;
  /**
   * Whether or not the tree of a search item is being loaded.
   */
  loadingSearchResultTree = false;
  /**
   * Observable that broadcasts whenever a new tree item is loaded.
   */
  $loadedTreeItem = new Subject();
  /**
   * Observable that broadcasts whenever the tree of a search result is ready.
   */
  $loadingSearchResultTree = new Subject<boolean>();
  $processedFile = new Subject();

  constructor(private oauth: OAuthTokenService, private files: FileService, private data: ExplorerDataService) {
    this.oauth.$userAuthenticated.subscribe(isAuthenticated => {
      if (isAuthenticated) {
        this.loadSearchResults();
      }
    });
    // Whenever a new tree item is loaded
    this.$loadedTreeItem.subscribe(() => {
      // Increase the amount of loaded items by one
      this.loadedTreeItems += 1;
      // If the loaded items match the total amount of items
      if (this.totalTreeItems === this.loadedTreeItems) {
        // The search result tree is no longer being loaded
        this.$loadingSearchResultTree.next(false);
      }
    });
    // Whenever the tree of a search result is loaded
    this.$loadingSearchResultTree.subscribe((val) => {
      // Update the status of the search result's tree
      this.loadingSearchResultTree = val;
    });
  }

  /**
 * Iterates recursively through the files tree request,
 * assigning their respective properties.
 *
 * @param {*} parent the item immmediately above in the hierarchy
 * @param {number} recursionLevel current level of recursion (ranges from 0 to fields length);
 * it's used to determine which fields of the array must be used to call the next layer of
 * recursion.
 * @param {number[]} fields an array with the tree's levels
 */
  preprocessResults(parent: any, recursionLevel: number, fields: string[]) {
    // If we're at the end of the recursive levels
    if (recursionLevel === fields.length) {
      return;
    }

    // Get the first and second positions of the fields array
    const parentFieldName = fields[recursionLevel];
    const childFieldName = fields[recursionLevel + 1];

    // Iterate over the parent's children
    parent[parentFieldName].forEach((child, childIndex) => {
      // If parent has a path
      if (parent['path']) {
        // Use it as base for the child path
        child['path'] = `${parent['path']} / ${parent['name']}`;
      } else if (parent['name']) {
        // Otherwise set the path as the parents name
        child['path'] = parent['name'];
      } else {
        // Otherwise, initialize it as empty
        child['path'] = '';
      }

      /**
       * To illustrate this point: consider the tree A > B > C.
       *
       * A has no parent, and therefore, has no parent.path nor parent.name.
       * Because of that, its path will be empty, meaning it's on a root level.
       *
       * B has a parent, and even though the parent has a name, it doesn't
       * have a path. So, unfortunately, we can't just simply join A's path
       * and name, because we'd have '${A.path} / ${A.name}' = ' / A',
       * with this unnecessary slash.
       *
       * Therefore, if B has a parent with name but with no path, B's path is
       * simply A.name = 'A', meaning it's inside of A.
       *
       * Finally, C has a parent with a name and path. So, you can just join
       * B's path and name to form '${B.path} / ${B.name}' = 'A / B'.
       *
       * If we had a fourth element, D, child of C, its path would be
       * '${C.path} / ${C.name}' = '{A / B} / {C}' = 'A / B / C'.
       */

      // Fetch the index of the child on parent
      child['id'] = parseInt(child.url.split('/')[child.url.split('/').length - 2]);
      child['index'] = childIndex;
      // Append the child's name to the path names
      child['pathNames'] = parent['pathNames'] ? parent['pathNames'].concat(child.name) : [child.name];

      let targetField = '';

      switch (recursionLevel) {
        case 0: targetField = 'units'; break;
        case 1: targetField = 'oilfields'; break;
        case 2: targetField = 'wells'; break;
        case 3: targetField = 'files'; break;
      }

      // Add this element to the searchable items
      this.searchableItems[targetField].push(child);

      // If this element has children
      if (childFieldName) {
        // Sort its children alphabetically
        child[childFieldName].sort((a, b) => a.name.localeCompare(b.name));
      }

      const itemParent = recursionLevel > 0 ? parent : null;
      this.data.postProcessItem(child, childFieldName, itemParent);

      child['pathIndexes'] = parent['pathIndexes'] ? parent['pathIndexes'].concat(child['index']) : [child['index']];

      // Go one level of recursion deeper
      this.preprocessResults(child, recursionLevel + 1, fields);
    });
  }

  /**
   * Fetch list of files as a tree from the API.
   */
  loadSearchResults() {
    // Perform a request passing the format parameter to list files as a tree
    this.files.getAll({ format: 'tree' }).then(response => {
      // Transfer the 'opunits' key to the 'units' key to keep thins consistent
      // Delete the 'opunits' key to reduce memory usage
      // List all the fields in the hierarchy
      const fields = ['tree', 'children', 'children', 'children'];
      // Initialize preprocessing
      this.preprocessResults(response, 0, fields);
      this.$processedFile.next(response);
      // When it's done, mark search as ready
      this.searchIsReady = true;
    });
  }

  /**
   * Searches for a particular term among the items from the search tree.
   *
   * @param {*} searchTerm the term to be searched
   * @param {KeyboardEvent} [evt] the keyboard event that triggered the function
   */
  search(searchTerm: any, evt?: KeyboardEvent) {
    // If the user cleared the input
    if (!searchTerm) {
      // They are not searching anymore
      this.isSearching = false;
      // There's no point in continuing the function
      return;
    }

    /**
     * Make the search term lower case to prevent having
     * to do this operation more than once inside a for loop.
     */
    searchTerm = searchTerm.toLowerCase();

    // If the function was not triggered by a keyboard event or if the pressed key is Enter
    if (!evt || evt.key === 'Enter') {
      // Engage search mode
      this.isSearching = true;

      // If the search tree has been loaded
      if (this.searchIsReady) {
        // Initialize the search results dict
        this.searchResults = {
          units: [],
          oilfields: [],
          wells: [],
          files: []
        };
        /**
         * Fetch the keys of the searchable items dict, which means the types of
         * hierarchy that may contain results (units, oilfields, etc).
         */
        const keys = Object.keys(this.searchableItems);
        // For each one of these keys
        for (const key of keys) {
          // Iterate through the searchable items of that key
          this.searchableItems[key].forEach(item => {
            /**
             * Check if it's a match. The item name is turned into
             * lower case because indexOf is case sensitive, and
             * as such, 'ba' would not be found inside 'Banana'.
             */
            if (item.name.toLowerCase().indexOf(searchTerm) >= 0) {
              // If it's a match, push it to the equivalent search results list
              this.searchResults[key].push(item);
            }
          });
        }
      }
    }
  }

  /**
   * Clears information regarding search.
   */
  clearSearch() {
    // Reset every key of the search results
    this.searchResults = {
      units: [],
      oilfields: [],
      wells: [],
      files: []
    };
    // Disable search mode
    this.isSearching = false;
  }
}
