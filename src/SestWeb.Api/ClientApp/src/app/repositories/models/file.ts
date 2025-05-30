/**
 * The structure of a file in an API response.
 */
export interface File {
    url: string,
    name: string,
    file_type: string,
    description: string,
    created_at: string,
    last_updated_at: string,
    well: {
        url: string,
        name: string
    },
    revision: FileRevision,
    revisions: string
}

/**
 * The structure of a file revision in an API response.
 */
export interface FileRevision {
    created_at: string,
    schema: string,
    author: {
        name: string,
        url: string
    },
    source: {
        name: string,
        url: string
    },
    description: string,
    content: any
}

/**
 * The structure of the payload needed to perform a POST
 * request on the /files/<id>/revisions endpoint.
 */
export interface FileRevisionData {
    description: string,
    content: any,
    schema?: string
}

/**
 * The structure of the payload needed to perform a POST
 * request on the /files/ endpoint.
 */
export interface FileData {
    name: string,
    description: string,
    fileType: string,
    content: any
    wellId: string | FileHierarchy
    schema?: string,
    updateOrCreate?: boolean,
}

/**
 * All valid parameters to pass to a GET request on the
 * /files/ endpoint.
 */
export interface FileListParams {
    format?: 'tree',
    file_type?: string,
    updated_after?: string,
    updated_before?: string
}

/**
 * The structure of a well when creating the hierarchy
 * in-place.
 */
export interface FileHierarchy {
    well: {
        opunit_name: string,
        oilfield_name: string,
        well_name: string,
        visible?: false
    }
}
