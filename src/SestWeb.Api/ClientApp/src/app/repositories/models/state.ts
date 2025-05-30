export enum NodeTypes {
    Perfil = '0',
    Folder = '1',
    Litologia = '2',
    Trajetória = '3',
    RegistroEvento = '4',
}

interface baseState {
    id?: string;
    fixa: boolean;
    name: string;
}

export interface Node extends baseState {
    tipo: NodeTypes;
    data: Node[] | Object;
}

export interface GraphicItem {
    id: string;
    idPoço: string;
    adicionadoTrend: boolean;
}

export interface GraphicArea {
    id: string;
    name: string;
    largura: number;
    items: GraphicItem[];
    maxX: number;
    minX: number;
    intervalo: number;
}

export interface Tab extends baseState {
    data: GraphicArea[];
}

export interface State {
    tree?: Node[];
    tabs: Tab[];
    idAbaAtual: string;
    profundidadeExibição: 'PM' | 'PV';
    posicaoDivisaoAreaGrafica: number;
}