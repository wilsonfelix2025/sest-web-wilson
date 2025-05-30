import { Injectable } from '@angular/core';
import * as HighchartsUtils from '@utils/highcharts';
import { Subscription } from 'rxjs';
import { TrendService } from 'app/repositories/trend.service';
import { DatasetService } from './dataset/dataset.service';
import { ProfileService } from 'app/repositories/profile.service';
import { Perfil } from 'app/repositories/models/perfil';
import { ProfileUtils } from '@utils/dataset/profile';
import { ProfileDatasetService } from './dataset/profile-dataset.service';
import { UNSET } from '@utils/vazio';
import { StateService } from './dataset/state/state.service';

@Injectable({
    providedIn: 'root'
})
export class EditTrendService {

    /**
     * Observable to watch when click occurred.
     */
    $clickChartObservable: Subscription;

    /**
     * Observable to watch when drag point has started.
     */
    $startDragPointObservable: Subscription;

    /**
     * Observable to watch when drag point is occurring.
     */
    $dragPointObservable: Subscription;

    /**
     * Observable to watch when drop point occurred.
     */
    $dropPointObservable: Subscription;

    constructor(
        private dataset: DatasetService,
        private trendService: TrendService,
        private perfisService: ProfileService,
        private profileDataset: ProfileDatasetService,
        private stateService: StateService,
    ) {
        this.$clickChartObservable = HighchartsUtils.clickChartObservable.subscribe(r => {
            // console.log('Clicou', r);
            try {
                // Verificar se está ligada a edição do grafico
                if (r.chart.userOptions.chart.zoomType === undefined) {
                    const series = r.chart.series.filter(el => el.name === 'Trend' || el.name === 'LBF' || el.userOptions.mnemonico === 'GPPI');
                    if (series.length > 0) {
                        const perfil: Perfil = this.dataset.getById(
                            series[0].userOptions.idPerfil ? series[0].userOptions.idPerfil : series[0].userOptions.id);
                        let pv = Math.round(r.event.xAxis[0].value);
                        const valor = Math.round(r.event.yAxis[0].value);

                        if (r.event.ctrlKey && !r.event.altKey && !r.event.shiftKey) {
                            // Verificar se esta fora dos limites
                            let sedimentos;
                            const poco = this.dataset.getById(this.dataset.currCaseId);;
                            if (poco.dadosGerais.geometria.categoriaPoço === 'OnShore') {
                                sedimentos = poco.dadosGerais.geometria.mesaRotativa + poco.dadosGerais.geometria.onShore.alturaDeAntePoço;
                            } else {
                                sedimentos = poco.dadosGerais.geometria.mesaRotativa + poco.dadosGerais.geometria.offShore.laminaDagua;
                            }
                            if (pv < sedimentos) {
                                pv = sedimentos;
                            } else if (pv > poco.trajetória.últimoPonto.pv.valor) {
                                pv = poco.trajetória.últimoPonto.pv.valor;
                            }

                            if (perfil.mnemonico === 'GPPI') {
                                this.adicionarPonto(perfil, pv, r.event.yAxis[0].value, r.chart, series);
                            } else {
                                this.adicionarTrecho(perfil, pv, valor, r.chart, series);
                            }

                        } else if (!r.event.ctrlKey && !r.event.altKey && r.event.shiftKey) {
                            if (perfil.mnemonico === 'GPPI') {
                                this.removerPonto(perfil, pv, valor, r.chart, series);
                            } else {
                                this.removerQuebra(perfil, pv, valor, r.chart, series);
                            }
                        }
                    }
                }
            } catch (error) {
                console.log('Erro clickChartObservable', r, error);
            }
        });
        this.$startDragPointObservable = HighchartsUtils.startDragPointObservable.subscribe(r => {
            // console.log('pegou ponto', r);
            try {
                if (r.point.series.userOptions.mnemonico !== 'GPPI') {
                    const yAxis = r.point.series.chart.yAxis[0];
                    r.point.series.options.dragDrop.dragMinY = yAxis.min;
                    r.point.series.options.dragDrop.dragMaxY = yAxis.max;

                    if (!r.event.ctrlKey && r.event.altKey && !r.event.shiftKey) {
                        r.point.wholeSerie = true;
                        r.point.series.options.dragDrop.draggableX = false;
                    } else {
                        r.point.wholeSerie = false;
                        r.point.series.options.dragDrop.draggableX = true;
                    }
                } else {
                    const maxX = r.point.series.xData[r.point.series.xData.length - 1];
                    const yAxis = r.point.series.chart.yAxis[0];
                    r.point.series.options.dragDrop.dragMinY = yAxis.min;
                    r.point.series.options.dragDrop.dragMaxY = yAxis.max;
                    r.point.series.options.dragDrop.dragMaxX = maxX;

                    const isEdgePoint = r.point.index === 0 || r.point.index === r.point.series.points.length - 1;

                    if (isEdgePoint || (!r.event.ctrlKey && r.event.altKey && !r.event.shiftKey)) {
                        r.point.wholeSerie = true;
                        r.point.series.options.dragDrop.draggableX = false;
                    } else {
                        r.point.wholeSerie = false;
                        r.point.series.options.dragDrop.draggableX = true;
                    }
                }
            } catch (error) {
                console.log('Erro startDragPointObservable', r, error);
            }

        });
        this.$dragPointObservable = HighchartsUtils.dragPointObservable.subscribe(r => {

            // console.log('Dragging ponto', r, r.point.series); // dragPlotYtmp
            try {
                if (r.point.series.userOptions.mnemonico !== 'GPPI') {
                    const initialPoint = r.event.origin.points[Object.keys(r.event.origin.points)[0]];
                    const newPoint = r.event.newPoint;

                    if (r.point.wholeSerie) {
                        r.point.series.points.forEach(point => {
                            if (r.point !== point) {
                                if (point.diference === undefined) {
                                    point.diference = initialPoint.y - point.y;
                                }
                                point.y = newPoint.y - point.diference;
                            }
                        });
                    }
                }
            } catch (error) {
                console.log('Erro dragPointObservable', r, error);
            }
        });
        this.$dropPointObservable = HighchartsUtils.dropPointObservable.subscribe(r => {
            // console.log('Dropou ponto', r);
            try {
                const series = r.point.series.chart.series.filter(el => el.name === 'Trend' || el.name === 'LBF');
                const gppis = r.point.series.chart.series.filter(el => el.userOptions.mnemonico === 'GPPI');
                if (series.length > 0) {
                    series.sort((a, b) => a.data[0].x - b.data[0].x).forEach((el, i) => { el.userOptions.indexTrecho = i; });
                    const perfil: Perfil = this.dataset.getById(series[0].userOptions.idPerfil);
                    const trecho = perfil.trend.trechos[r.point.series.userOptions.indexTrecho];

                    this.moverPonto(perfil, trecho, r.point.series, series, r.point.wholeSerie);
                } else if (gppis.length > 0) {
                    const perfil: Perfil = this.dataset.getById(gppis[0].userOptions.id);
                    this.moverPontoPerfil(perfil, r.point);
                }
            } catch (error) {
                console.log('Erro dropPointObservable', r, error);
            }
        });
    }

    adicionarPonto(perfil: Perfil, profundidade, valor, chart, series) {
        let i = 0;
        series = series[0];

        // No point can be created before the first point
        if (profundidade >= series.points[series.points.length - 1].x) {
            return;
        }

        // No point can be added after the last point
        if (profundidade <= series.points[0].x) {
            return;
        }

        // Iterate through the series to find the two best points to use for interpolation
        for (i = 0; i < series.points.length; i++) {
            // Alias for a point
            const point = series.points[i];

            // If the click was after this point's x, it and the previous point will be used
            if (point.x > profundidade) {
                break;
            }
        }

        let arr = series.points.slice(0, i).map(point => [point.x, point.y]);
        arr.push([profundidade, valor]);
        arr = arr.concat(series.points.slice(i, series.points.length).map(point => [point.x, point.y]));
        series.setData([]);
        series.setData(arr);
        const novoPonto = {
            pm: { valor: this.stateService.currentDeepView === 'PM' ? profundidade : 0 },
            pv: { valor: this.stateService.currentDeepView === 'PV' ? profundidade : 0 },
            valor: valor,
            origem: 'Interpretado'
        };
        perfil.pontos.push(novoPonto);
        this.perfisService.edit(perfil, this.stateService.currentDeepView).then(resp => {
            this.profileDataset.update(resp.perfil);

            if (!UNSET(resp.perfisAlterados)) {
                this.profileDataset.updateList(resp.perfisAlterados);
            }
        });
        chart.redraw();
    }

    moverPontoPerfil(perfil: Perfil, ponto) {
        const profundidade = ponto.x;
        const novoValor = ponto.y;
        const pontoIndex = ponto.index;

        perfil.pontos[pontoIndex] = {
            pm: { valor: this.stateService.currentDeepView === 'PM' ? profundidade : 0 },
            pv: { valor: this.stateService.currentDeepView === 'PV' ? profundidade : 0 },
            valor: novoValor,
            origem: 'Interpretado'
        };
        this.perfisService.edit(perfil, this.stateService.currentDeepView).then(resp => {
            this.profileDataset.update(resp.perfil);

            if (!UNSET(resp.perfisAlterados)) {
                this.profileDataset.updateList(resp.perfisAlterados);
            }
        });
    }

    removerPonto(perfil: Perfil, profundidade, valor, chart, series) {
        let i = 0;
        series = series[0];

        // No point can be created before the first point
        if (profundidade > series.points[series.points.length - 1].x) {
            return;
        }

        // No point can be added after the last point
        if (profundidade <= series.points[0].x) {
            return;
        }

        if (series.points.length === 2) {
            return;
        }

        // Iterate through the series to find the two best points to use for interpolation
        for (i = 0; i < series.points.length; i++) {
            // Alias for a point
            const point = series.points[i];

            // If the click was after this point's x, it and the previous point will be used
            if (point.x > profundidade) {
                if (Math.abs(point.x - profundidade) > 50) {
                    if (Math.abs(series.points[i - 1].x - profundidade) > 50) {
                        return;
                    } else {
                        i = i - 1;
                        break;
                    }
                } else {
                    break;
                }
            }
        }

        const arr = series.points.map(point => [point.x, point.y]);
        arr.splice(i, 1);
        series.setData([]);
        series.setData(arr);
        perfil.pontos.splice(i, 1);
        this.perfisService.edit(perfil, 'PV').then(resp => {
            this.profileDataset.update(resp.perfil);

            if (!UNSET(resp.perfisAlterados)) {
                this.profileDataset.updateList(resp.perfisAlterados);
            }
        });
        chart.addSeries(series as Highcharts.SeriesLineOptions);
        chart.redraw();
    }

    adicionarTrecho(perfil: Perfil, pv, valor, chart, series) {
        // Encontrar o trecho que será adicionado a quebra
        const trechoIndex = perfil.trend.trechos.findIndex(el => el.pvTopo < pv && el.pvBase >= pv);

        // console.log('PERFIL', perfil.trend);
        if (trechoIndex >= 0) {
            // Criar novo trecho com base igual do trecho antigo
            const novoTrecho = {
                pvTopo: pv,
                valorTopo: valor,
                pvBase: perfil.trend.trechos[trechoIndex].pvBase,
                valorBase: perfil.trend.trechos[trechoIndex].valorBase,
                inclinação: 0
            };
            this.calcularInclinacao(novoTrecho);

            // Mudar a base do trecho antigo para o topo do novo trecho
            perfil.trend.trechos[trechoIndex].pvBase = pv;

            // Mudar a base do trecho antigo para o topo do novo trecho
            perfil.trend.trechos[trechoIndex].valorBase = valor;
            this.calcularInclinacao(perfil.trend.trechos[trechoIndex]);

            perfil.trend.trechos.splice(trechoIndex + 1, 0, novoTrecho);

            // Encontrar o trecho no grafico
            const serieAntiga = series.find(el => el.data[0].x < pv && el.data[1].x >= pv);

            // Gerar os graficos dos novos trechos
            ProfileUtils.trendPointsToData(perfil.trend, perfil.id, this.dataset.getById(this.dataset.currCaseId));

            // Remover do grafico
            serieAntiga.remove();

            // Adicionar os trechos novos no gráfico
            chart.addSeries(perfil.trend.series[trechoIndex] as Highcharts.SeriesLineOptions);
            chart.addSeries(perfil.trend.series[trechoIndex + 1] as Highcharts.SeriesLineOptions);
        } else {
            let index, novoTrecho;
            if (perfil.trend.trechos[0].pvTopo > pv) {
                //  se for acima dos trechos atuais
                index = 0;

                // Criar novo trecho com base igual ao topo do trecho inicial
                novoTrecho = {
                    pvTopo: pv,
                    valorTopo: valor,
                    pvBase: perfil.trend.trechos[index].pvTopo,
                    valorBase: perfil.trend.trechos[index].valorTopo,
                    inclinação: 0
                };
            } else {
                // se for abaixo dos trechos atuais
                index = perfil.trend.trechos.length;

                // Criar novo trecho com topo igual a base do trecho final
                novoTrecho = {
                    pvTopo: perfil.trend.trechos[index - 1].pvBase,
                    valorTopo: perfil.trend.trechos[index - 1].valorBase,
                    pvBase: pv,
                    valorBase: valor,
                    inclinação: 0
                };
            }
            this.calcularInclinacao(novoTrecho);
            perfil.trend.trechos.splice(index, 0, novoTrecho);

            // Gerar o grafico do novo trecho
            ProfileUtils.trendPointsToData(perfil.trend, perfil.id, this.dataset.getById(this.dataset.currCaseId));

            // Adiciona o trecho novo no gráfico
            chart.addSeries(perfil.trend.series[index] as Highcharts.SeriesLineOptions);
        }

        this.salvarTrend(perfil);
    }

    moverPonto(perfil: Perfil, trecho, serieAtual, series, moveuLinha) {
        // Se não moveu um trecho inteiro
        if (!moveuLinha) {
            // Se moveu o ponto topo
            if (trecho.valorTopo !== serieAtual.points[0].y || trecho.pvTopo !== serieAtual.points[0].x) {
                // Se tiver um trecho acima
                if (serieAtual.userOptions.indexTrecho > 0) {
                    const _trecho = perfil.trend.trechos[serieAtual.userOptions.indexTrecho - 1];
                    // recalcula apenas o valor da base, a partir do topo, para a mesma inclinação
                    const corte = this.calcularCorte(_trecho.pvTopo, _trecho.pvBase, _trecho.valorTopo,
                        _trecho.valorBase, serieAtual.points[0].x, false);

                    _trecho.pvBase = serieAtual.points[0].x;
                    _trecho.valorBase = _trecho.valorTopo - corte;
                    series[serieAtual.userOptions.indexTrecho - 1].data[1].update({ x: serieAtual.points[0].x, y: _trecho.valorBase });
                }
                trecho.pvTopo = serieAtual.points[0].x;
            } else { // Se moveu o ponto base
                // Se tiver um trecho abaixo dele
                if (serieAtual.userOptions.indexTrecho < perfil.trend.trechos.length - 1) {
                    const _trecho = perfil.trend.trechos[serieAtual.userOptions.indexTrecho + 1];
                    // recalcula apenas o valor de topo, a partir da base, para a mesma inclinação
                    const corte = this.calcularCorte(_trecho.pvTopo, _trecho.pvBase, _trecho.valorTopo,
                        _trecho.valorBase, serieAtual.points[1].x, true);

                    _trecho.pvTopo = serieAtual.points[1].x;
                    _trecho.valorTopo = corte + _trecho.valorBase;
                    series[serieAtual.userOptions.indexTrecho + 1].data[0].update({ x: serieAtual.points[1].x, y: _trecho.valorTopo });
                }
                trecho.pvBase = serieAtual.points[1].x;
            }
        }
        trecho.valorTopo = serieAtual.points[0].y;
        trecho.valorBase = serieAtual.points[1].y;
        this.calcularInclinacao(trecho);

        this.salvarTrend(perfil);
    }

    removerQuebra(perfil: Perfil, pv, valor, chart, series) {
        const rangePv = (perfil.ultimoPonto.pv.valor - perfil.primeiroPonto.pv.valor) * 0.1;
        const rangeValor = (perfil.larg.max - perfil.larg.min) * 0.1;

        const indexTrechoTopo = perfil.trend.trechos.findIndex(el => el.pvTopo + rangePv > pv && el.pvTopo - rangePv < pv &&
            el.valorTopo + rangeValor > valor && el.valorTopo - rangeValor < valor);
        const indexTrechoBase = perfil.trend.trechos.findIndex(el => el.pvBase + rangePv > pv && el.pvBase - rangePv < pv &&
            el.valorBase + rangeValor > valor && el.valorBase - rangeValor < valor);
        if (indexTrechoBase >= 0 || indexTrechoTopo >= 0) {
            // Se so tem um trecho, simplesmente apaga o trend
            if (perfil.trend.trechos.length <= 1) {
                this.removerTrechoDoGrafico(perfil.trend.trechos[0], series);
                this.removerTrend(perfil);
                return;
            }

            if (indexTrechoTopo === 0) {
                // Se clicou no primeiro ponto, simplesmente remove o primeiro trecho
                this.removerTrechoDoGrafico(perfil.trend.trechos[indexTrechoTopo], series);
                perfil.trend.trechos.splice(indexTrechoTopo, 1);
            } else if (indexTrechoBase === perfil.trend.trechos.length - 1) {
                // Se clicou no ultimo ponto, simplesmente remove o ultimo trecho
                this.removerTrechoDoGrafico(perfil.trend.trechos[indexTrechoBase], series);
                perfil.trend.trechos.splice(indexTrechoBase, 1);
            } else {
                // Se clicou num ponto intermediario
                // Usa prioritariamente o trecho base clicado
                const indexTrecho = indexTrechoBase < 0 ? indexTrechoTopo : indexTrechoBase;

                const trechoAtual = perfil.trend.trechos[indexTrecho];
                let proximoTrecho;
                if (indexTrechoBase < 0) {
                    proximoTrecho = perfil.trend.trechos[indexTrecho - 1];
                    trechoAtual.pvTopo = proximoTrecho.pvTopo;
                } else {
                    proximoTrecho = perfil.trend.trechos[indexTrecho + 1];
                    trechoAtual.pvBase = proximoTrecho.pvBase;
                }

                // Remove trecho atual
                this.removerTrechoDoGrafico(trechoAtual, series);

                // Atualiza ultimo ponto do trecho atual
                trechoAtual.valorBase = proximoTrecho.valorBase;
                this.calcularInclinacao(trechoAtual);

                // Remove proximo trecho
                this.removerTrechoDoGrafico(proximoTrecho, series);
                if (indexTrechoBase < 0) {
                    perfil.trend.trechos.splice(indexTrecho - 1, 1);
                } else {
                    perfil.trend.trechos.splice(indexTrecho + 1, 1);
                }

                // Gerar o grafico do trecho atualizado
                ProfileUtils.trendPointsToData(perfil.trend, perfil.id, this.dataset.getById(this.dataset.currCaseId));

                // Adicionar trecho atualizado no grafico
                chart.addSeries(perfil.trend.series[indexTrecho] as Highcharts.SeriesLineOptions);
            }
            this.salvarTrend(perfil);
        }

    }

    removerTrechoDoGrafico(trecho, series) {
        // Encontrar o trecho no grafico
        const serie = series.find(el => el.remove !== null && el.data[0].x === trecho.pvTopo && el.data[1].x === trecho.pvBase
            && el.data[0].y === trecho.valorTopo && el.data[1].y === trecho.valorBase);

        if (serie !== undefined) {
            // Remover do grafico
            serie.remove();
        }
    }

    calcularInclinacao(trecho) {
        if (trecho.valorBase === trecho.valorTopo) {
            trecho.inclinação = 0;
        } else {
            trecho.inclinação = (trecho.pvBase - trecho.pvTopo) / (trecho.valorBase - trecho.valorTopo);
        }
    }

    salvarTrend(perfil) {
        setTimeout(() => {
            this.trendService.edit(perfil).then(res => {
                if (!UNSET(res.perfisAlterados)) {
                    this.profileDataset.updateList(res.perfisAlterados);
                }
            });
            this.profileDataset.editTrend(perfil.id);
        }, 1);
    }

    removerTrend(perfil) {
        setTimeout(() => {
            this.trendService.remove(perfil.id).then(res => {
                if (!UNSET(res.perfisAlterados)) {
                    this.profileDataset.updateList(res.perfisAlterados);
                }
            });
            this.profileDataset.removeTrend(perfil.id);
            perfil.trend = undefined;
        }, 1);
    }

    calcularCorte(pvTopo: number, pvBase: number, valorTopo: number, valorBase: number, pvNovo: number, mudouTopo: boolean) {
        const b = valorTopo - valorBase;
        const c = pvTopo - pvBase;
        let z;
        if (mudouTopo) {
            z = pvNovo - pvBase;
        } else {
            z = pvTopo - pvNovo;
        }

        return (b * z / c);
    }

    calcularValor(trecho) {
        if (trecho.inclinação === 0) {
            return 0;
        } else {
            return (trecho.pvBase - trecho.pvTopo) / trecho.inclinação;
        }
    }
}
