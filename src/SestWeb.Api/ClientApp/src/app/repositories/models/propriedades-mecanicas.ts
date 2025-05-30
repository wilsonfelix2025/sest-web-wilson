import { GrupoLitologico } from './litologia';
import { CorrelationSlim } from './correlation';

export interface CorrelaçõesPossíveis {
    grupoLitologico: GrupoLitologico;
    ucsPossibleCorrelations: CorrelationSlim[];
    coesaPossibleCorrelations: CorrelationSlim[];
    angatPossibleCorrelations: CorrelationSlim[];
    restrPossibleCorrelations: CorrelationSlim[];
    biotPossibleCorrelations: CorrelationSlim[];
}
export interface CorrelaçõesSelecionadas {
    idCaso: string;
    grupoLitologico: string;
    ucs: string;
    coesa: string;
    angat: string;
    restr: string;
    biot: string;
}
