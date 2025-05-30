export interface Export {
  idPoço: string;
  perfis: string[];
  pmTopo: number;
  pmBase: number;
  intervalo: number;
  trajetoria: boolean;
  litologia: boolean;
  pv: boolean;
  cota: boolean;
  arquivo: 'CSV' | 'LAS';
}


export interface RelatorioRequest {
  trajetoria: boolean;
  litologia: boolean;
  estratigrafias: boolean;
  graficos: string[];
}

export interface Grafico {
  titulo: string;
  data;
  curvas: string[];
  registros: string[];
}

export interface Relatorio {
  idPoço: string;
  trajetoria?: Grafico;
  litologia?: Grafico;
  estratigrafias?: Grafico[];
  graficos: Grafico[];
  nome?: boolean;
  tipo?: boolean;
  profundidadeFinal?: boolean;
  alturaMR?: boolean;
  lda?: boolean;
  extensao: 'JPEG' | 'PDF';
}
