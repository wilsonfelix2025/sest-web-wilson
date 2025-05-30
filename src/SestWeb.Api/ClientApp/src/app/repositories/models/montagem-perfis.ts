export interface Trecho {
    idPoçoApoio: string;
    pvTopoApoio: number;
    pvBaseApoio: number;
    pvTopo: number;
    pvBase: number;
    idRhobApoio?: string;
    idDTCApoio?: string;
    idDTSApoio?: string;
    idGRayApoio?: string;
    idResistApoio?: string;
    idNPhiApoio?: string;
}

export interface MontagemPerfis {
    listaTrechos: Trecho[];
    idPoço: string;
    nomeRhob?: string;
    nomeDTC?: string;
    nomeDTS?: string;
    nomeGRay?: string;
    nomeResist?: string;
    nomeNPhi?: string;
    removerTendência: boolean;
}
