import { Area, Geometria, Identificação } from 'app/repositories/models/case';
import { Objetivo, Sapata, PointTrajetoria } from 'app/repositories/models/trajetoria';

interface File {
  filePath: string;
  fields: { [key: string]: boolean };
  fixedDeepType: boolean;
  hasFields: boolean;
}

export interface FileData extends File {
  profiles: {
    name: string;
    mnemonic?: string;
    type?: string;
    unit: string;
  }[];
  lithologies: {
    name: string;
  }[];
  trajectories: {
    name: string;
  }[];
  extras?: {
    poçoWeb: { poçoWeb: boolean };
    wells: {};
    selectedWellName?: '';
  };
}

interface ObjToImport {
  ação: 'Novo' | 'Acrescentar' | 'Sobrescrever';
  nome: string;
  novoNome?: string;
  tipo: string;
  valorTopo?: number;
  valorBase?: number;
}

export interface LithologyToImport extends ObjToImport { }
export interface ProfileToImport extends ObjToImport {
  unidade: string;
}

export interface ImportData {
  caminhoDoArquivo: string;
  poçoId: string;

  tipoProfundidade: 'PM' | 'Cota';
  correçãoMesaRotativa?: number;

  dadosSelecionados: string[];
  listaPerfis: ProfileToImport[];
  listaLitologias: LithologyToImport[];

  extras?: { [key: string]: string };
}

interface LitologiaData extends LithologyToImport {
  tipoProfundidade: 'PM' | 'Cota';
  correçãoMesaRotativa?: number;

  pontosLitologia: {
    pm: number;
    tipoRocha: string;
  }[];
}

export interface ImportLitologiaData {
  poçoId: string;
  litologia: LitologiaData;
}

export interface ImportTrajetoriaData {
  poçoId: string;
  pontosTrajetória: PointTrajetoria[];
}

interface PerfilData extends ProfileToImport {
  tipoProfundidade: 'PM' | 'Cota';
  correçãoMesaRotativa?: number;

  pontosPerfil: {
    pm: number;
    valor: number;
  }[];
}

export interface ImportPerfilData {
  poçoId: string;
  perfis: PerfilData[];
}

export interface CursorInfo {
  y: number;
  graphs: {
    x: number;
    title: string;
  }[];
  unidade?: string;
  isBaseChart: boolean;
}

export interface UpdateDadosGerais {
  geometria: Geometria;
  éVertical: boolean;
  pmFinal: number;
  identificação: Identificação;
  area: Area;
  profundidadeReferênciaSapata: string;
  sapatas: Sapata[];
  profundidadeReferênciaObjetivo: string;
  objetivos: Objetivo[];
  profundidadeReferênciaEstratigrafia: string;
  estratigrafias: Estratigrafia[];
}

export interface Estratigrafia {
  profundidadeValor: number;
  tipo: TipoEstratigrafia;
  sigla: string;
  descrição: string;
}

enum TipoEstratigrafia {
  OU, // 'Outros',
  FM, // 'Formação',
  CR, // 'Cronoestratigráfica',
  MB, // 'Membro',
  IN, // 'Indefinido',
  MC, // 'Marco',
  GR, // 'Grupo',
  CM, // 'Camada'
}

export const estratigrafiaTypes = {
  OU: 'Outros',
  FM: 'Formação',
  CR: 'Cronoestratigráfica',
  MB: 'Membro',
  IN: 'Indefinido',
  MC: 'Marco',
  GR: 'Grupo',
  CM: 'Camada',
};

export const estratigrafiaTypesSigla = {
  Outros: 'OU',
  Formação: 'FM',
  Cronoestratigráfica: 'CR',
  Membro: 'MB',
  Indefinido: 'IN',
  Marco: 'MC',
  Grupo: 'MC',
  Camada: 'CM',
};

export interface ProfileInitialData {
  initialPoint: number;
  finalPoint: number;
  initialValue: number;
  profileId: string;
  mnemonic: string;
  name: string;
  unit: string;
}

export interface ObjInitialData {
  initialWellPoint: number;
  finalWellPoint: number;
  profiles: ProfileInitialData[];
}

export interface InsertInitialData {
  perfilId: string;
  nomeNovoPerfil: string;
  tipoDeTrecho: string;
  tipoTratamento: string;
  pmLimite: number;
  litologiasSelecionadas: string[];
}
