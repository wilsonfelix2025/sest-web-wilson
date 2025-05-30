export interface EstiloRegistroEvento {
    marcador: string;
    corDoMarcador: string;
    contornoDoMarcador: string;
}

interface PontoRegistro {
    pm: { valor: number };
    pv: { valor: number };
}
export interface TrechoRegistro {
    valor: number;
    ponto: PontoRegistro;
    comentário?: string;
}

export interface TrechoEvento {
    topo: PontoRegistro;
    base: PontoRegistro;
    comentário: string;
}

interface RegistroEvento {
    id: string;
    idPoço: string;
    nome: string;
    estiloVisual: EstiloRegistroEvento;
}
export interface Registro extends RegistroEvento {
    tipo: 'Registro';
    unidade: string;
    trechos: TrechoRegistro[];
}
export interface Evento extends RegistroEvento {
    tipo: 'Evento';
    valorPadrão: number;
    trechos: TrechoEvento[];
}

export interface Marcador {
    name: string;
    value?: string;
    avatar?: string;
    disabled?: boolean;
    fixo?: boolean;
}

export const MARCADORES: Marcador[] = [
    { name: 'Marcadores Customizáveis', disabled: true },
    { value: 'Circulo', name: 'Circulo', avatar: 'assets/images/custom/circulo.png' },
    { value: 'Diamante', name: 'Diamante', avatar: 'assets/images/custom/diamante.png' },
    { value: 'Quadrado', name: 'Quadrado', avatar: 'assets/images/custom/quadrado.png' },
    { value: 'Triangulo', name: 'Seta p/ direita', avatar: 'assets/images/custom/triangulo.png' },
    { value: 'Triangulo-invertido', name: 'Seta p/ esquerda', avatar: 'assets/images/custom/triangulo-invertido.png' },
    { name: 'Marcadores Fixos', disabled: true },
    { value: 'Dois-circulos', name: 'Dois círculos', fixo: true, avatar: 'assets/images/custom/pointball.png' },
    { value: 'Triangulo-circulo', name: 'Triangulo-circulo', fixo: true, avatar: 'assets/images/custom/triangleball.png' },
    { value: 'Cruz', name: 'Cruz', fixo: true, avatar: 'assets/images/custom/cross-1.png' },
    { value: 'Pescaria', name: 'Pescaria', fixo: true, avatar: 'assets/images/custom/fish-1.png' },
    { value: 'Sol-rosa', name: 'Sol rosa', fixo: true, avatar: 'assets/images/custom/sol-rosa.png' },
    { value: 'Sol-laranja', name: 'Sol laranja', fixo: true, avatar: 'assets/images/custom/sol-laranja.png' },
    { value: 'Círculo-dividido', name: 'Círculo dividido', fixo: true, avatar: 'assets/images/custom/circulo-dividido.png' },
    { value: 'Ponto-circulo', name: 'Ponto e círculo', fixo: true, avatar: 'assets/images/custom/ponto-circulo.png' },
    { value: 'Zig-Zag', name: 'Zig Zag', fixo: true, avatar: 'assets/images/custom/zig-zag.png' },
];

interface Update {
    idRegistroEvento: string;
}
export interface TrechoRegistroUpdate extends Update {
    pm: number;
    pv: number;
    valor: number;
    unidade?: string;
    comentário?: string;
};
export interface TrechoEventoUpdate extends Update {
    pmTopo: number;
    pmBase: number;
    comentário: string;
};
export interface EstiloUpdate extends Update, EstiloRegistroEvento {
    unidade?: string;
    valorPadrão?: number;
}
