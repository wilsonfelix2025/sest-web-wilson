export function iconFormatter(type: importType, top?: number, bottom?: number) {
    let tooltip = translateImportType[type];
    let moreInfo = '';
    if (type === 'new') {
        tooltip = 'Criar Novo';
    } else if (type === 'append') {
        tooltip = 'Adicionar em existente';
    } else {
        tooltip = 'Sobrescrever'
    }
    if (top !== undefined && bottom !== undefined) {
        moreInfo = `top='${top}' bottom='${bottom}'`
    }

    return `<img src='assets/images/icons/${type}.svg' alt='Filtro' style='opacity: 50%;width: 21px; height: 21px; margin-top:-4px;vertical-align: middle;' title='${tooltip}' id='${type}' ${moreInfo}>`
}

export type importType = 'new' | 'append' | 'overwrite';
export const translateImportType = {
    'new': 'Novo',
    'append': 'Acrescentar',
    'overwrite': 'Sobrescrever'
}