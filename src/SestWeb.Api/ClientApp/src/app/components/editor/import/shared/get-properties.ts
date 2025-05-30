const typeRegex = /(.*?).</;
const idRegex = /id=["'](.+?)["']/;
const topRegex = /top=["']((\d|\.|,)+)["']/;
const bottomRegex = /bottom=["']((\d|\.|,)+)["']/;

export function getProperties(nameWithIcon: string, typeWithIcon: string, unit: string, tableName: string) {
    const name = nameWithIcon.match(idRegex)[1];

    const typeDescription = typeWithIcon.match(typeRegex)[1];

    const importType = typeWithIcon.match(idRegex)[1];

    const topValue = typeWithIcon.match(topRegex);

    const bottomValue = typeWithIcon.match(bottomRegex);

    const obj = {
        table: tableName,
        name: name,
        importType: importType,
        typeDescription: typeDescription,
        unit: unit,
        top: undefined,
        bottom: undefined,
    }

    if (topValue !== null && topValue[1] !== undefined && bottomValue !== null && bottomValue[1] !== undefined) {
        obj.top = Number(topValue[1]);
        obj.bottom = Number(bottomValue[1]);
    }

    return obj;
}


export function getRowProperties(table, row: number, tableName: string) {
    const iconHtml = table[row][1];

    const importType = iconHtml.match(idRegex)[1];

    const topValue = iconHtml.match(topRegex);

    const bottomValue = iconHtml.match(bottomRegex);

    const obj: any = {
        table: tableName,
        importType: importType,
        position: row,
        name: table[row][2],
        type: table[row][3],
        context: this
    };

    if (topValue !== null && topValue[1] !== undefined && bottomValue !== null && bottomValue[1] !== undefined) {
        obj.top = topValue[1] as number;
        obj.bottom = bottomValue[1] as number;
    }

    return obj;
}