export interface GrupoPerfil {
    nome: string;
    descrição: string;
}

export const GRUPO_PERFIS = {
    genérico: { nome: 'Genérico', descrição: 'Grupo genérico' },
    grupoPerfil: { nome: 'Perfis', descrição: 'Perfis' },
    grupoPropMec: { nome: 'PropriedadesMecânicas', descrição: 'Propriedades Mecânicas' },
    grupoPressões: { nome: 'Pressões', descrição: 'Pressões' },
    grupoTensões: { nome: 'Tensões/Pressões', descrição: 'Tensões/Pressões' },
    grupoGradientes: { nome: 'Gradientes', descrição: 'Gradientes' },
};
