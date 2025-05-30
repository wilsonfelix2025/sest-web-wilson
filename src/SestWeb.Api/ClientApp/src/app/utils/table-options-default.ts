export class TableOptions {
    /**
     * Opções de configurações usada em todas as tabelas nos sest-web.
     */
    static readonly TABLE_DEFAULT_OPTIONS = {
        invalidCellClassName: 'color-red',
        className: 'htCenter htMiddle',
        stretchH: 'all',
        dropdownMenu: false,
        manualRowMove: false,
        manualColumnMove: false,
        trimDropdown: false,
        comments: {
            readOnly: true
        },
        copyPaste: {
            columnsLimit: 1000,
            rowsLimit: 500000,
            pasteMode: 'overwrite',
        },
    };

    /**
     * Criar um objeto com as configurações padrão, acrescentando os parametros passados.
     *
     * @param parameters um objeto com as configurações adicionais para a tabela.
     */
    static createDefault(parameters: any, border?: boolean) {
        if (border) {
            return Object.assign(parameters, this.TABLE_DEFAULT_OPTIONS);
        }
        return Object.assign(parameters, this.TABLE_DEFAULT_OPTIONS, { className: 'htCenter htMiddle no-border-table' });
    }

    static removeError(table, row, prop) {
        if (table) {
            const comment = table.getPlugin('comments');
            const col = table.propToCol(prop)

            comment.removeCommentAtCell(row, col);
            table.setCellMetaObject(row, col, { valid: true });
        }
    }

    static setError(table, row, prop, error) {
        if (table) {
            const comment = table.getPlugin('comments');
            const col = table.propToCol(prop)

            comment.setCommentAtCell(row, col, error);
            comment.updateCommentMeta(row, col, { readOnly: true });
            table.setCellMetaObject(row, col, { valid: false });
        }
    }
}