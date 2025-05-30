import { Point } from './case';

export interface PointTrajetoria extends Point {
    azimute: number;
    inclinação: number;
    // ew: number;
    // ns: number;
    // dls: number;
    // polCoordDispl: number;
    // polCoordDirec: number;
    pmProj: number;
    pvProj: number;
}

export interface Sapata {
    pv?: number;
    pm: number;
    diâmetro: number;
}

export interface Objetivo {
    pv?: number;
    pm: number;
    tipoObjetivo: string;
}

interface Canvas {
    data: number[][];
    pointStart: number;
    pointEnd: number;
    color: string;
}

export interface Trajetoria {
    éVertical: boolean;
    métodoDeCálculoDaTrajetória: string;
    pontos: PointTrajetoria[];
    // pontosProjeção: { profundidade: number, valor: number }[];
    primeiroPonto: PointTrajetoria;
    últimoPonto: PointTrajetoria;
    count: number;
    canvas: Canvas;
}
