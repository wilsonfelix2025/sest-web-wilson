import { Component, OnInit, Input } from '@angular/core';
import { ColorPickerService } from 'ngx-color-picker';

@Component({
    selector: 'sest-edit-profile-style',
    templateUrl: './edit-profile-style.component.html',
    styleUrls: ['./edit-profile-style.component.scss']
})
export class EditProfileStyleComponent implements OnInit {

    @Input() perfil: any;

    nome: { value: string, tooltip: string } = { value: '', tooltip: '' };
    descricao: { value: string, tooltip: string } = { value: '', tooltip: '' };
    mnemonico: { value: string, tooltip: string } = { value: '', tooltip: '' };
    unidade: { value: string, tooltip: string } = { value: '', tooltip: '' };

    corLinha: { value: string, tooltip: string } = { value: '', tooltip: '' };
    espessura: { value: string, tooltip: string } = { value: '', tooltip: '' };
    estilo: { value: string, tooltip: string } = { value: '', tooltip: '' };
    marcador: { value: string, tooltip: string } = { value: '', tooltip: '' };
    corMarcador: { value: string, tooltip: string } = { value: '', tooltip: '' };

    marcadores: Object[] = [
        { id: 0, name: 'Nenhum', avatar: '', value: 'Nenhum' },
        { id: 1, name: 'Circulo', avatar: 'assets/images/custom/circulo.png', value: 'Circulo' },
        { id: 2, name: 'Diamante', avatar: 'assets/images/custom/diamante.png', value: 'Diamante' },
        { id: 3, name: 'Quadrado', avatar: 'assets/images/custom/quadrado.png', value: 'Quadrado' },
        { id: 4, name: 'Seta p/ direita', avatar: 'assets/images/custom/triangulo.png', value: 'Triangulo' },
        { id: 5, name: 'Seta p/ esquerda', avatar: 'assets/images/custom/triangulo-invertido.png', value: 'TrianguloInvertido' }
    ];

    espessuras: Object[] = [
        { id: 1, name: 'Espessura-1', avatar: 'assets/images/custom/espessura-1.png' },
        { id: 2, name: 'Espessura-2', avatar: 'assets/images/custom/espessura-2.png' },
        { id: 3, name: 'Espessura-3', avatar: 'assets/images/custom/espessura-3.png' },
        { id: 4, name: 'Espessura-4', avatar: 'assets/images/custom/espessura-4.png' },
        { id: 5, name: 'Espessura-5', avatar: 'assets/images/custom/espessura-5.png' }
    ];

    estilos: Object[] = [
        { id: 1, name: 'Solid', avatar: 'assets/images/custom/estilo-1.png' },
        { id: 2, name: 'ShortDash', avatar: 'assets/images/custom/estilo-2.png' },
        { id: 3, name: 'ShortDot', avatar: 'assets/images/custom/estilo-3.png' },
        { id: 4, name: 'ShortDashDot', avatar: 'assets/images/custom/estilo-4.png' },
        { id: 5, name: 'ShortDashDotDot', avatar: 'assets/images/custom/estilo-5.png' },
        { id: 6, name: 'Dot', avatar: 'assets/images/custom/estilo-6.png' },
        { id: 7, name: 'Dash', avatar: 'assets/images/custom/estilo-7.png' },
        { id: 8, name: 'LongDash', avatar: 'assets/images/custom/estilo-8.png' },
        { id: 9, name: 'DashDot', avatar: 'assets/images/custom/estilo-9.png' },
        { id: 10, name: 'LongDashDot', avatar: 'assets/images/custom/estilo-10.png' },
        { id: 11, name: 'LongDashDotDot', avatar: 'assets/images/custom/estilo-11.png' }
    ];

    constructor(
        private cpService: ColorPickerService,
    ) { }

    ngOnInit() {
        this.nome.value = this.perfil.nome;
        this.descricao.value = this.perfil.descrição;
        this.mnemonico.value = this.perfil.mnemonico;
        this.unidade.value = this.perfil.grupoDeUnidades.unidadePadrão.símbolo;

        this.corLinha.value = this.perfil.estiloVisual.corDaLinha;
        this.espessura.value = `Espessura-${this.perfil.estiloVisual.espessura}`;
        this.estilo.value = this.perfil.estiloVisual.estiloLinha;
        this.marcador.value = this.perfil.estiloVisual.marcador;
        this.corMarcador.value = this.perfil.estiloVisual.corDoMarcador;
    }

    public onChangeColorHex8(color: string): string {
        const hsva = this.cpService.stringToHsva(color, true);

        if (hsva) {
            return this.cpService.outputFormat(hsva, 'rgba', null);
        }

        return '';
    }

    commitVisualStyle() {
        this.perfil.nome = this.nome.value;
        this.perfil.name = this.nome.value;
        this.perfil['descrição'] = this.descricao.value;
        this.perfil.estiloVisual.corDaLinha = this.corLinha.value;
        this.perfil.estiloVisual.espessura = this.espessura.value ? parseInt(this.espessura.value.replace(/Espessura-/g, '')) : 0;
        this.perfil.estiloVisual.estiloLinha = this.estilo.value ? this.estilo.value : '';
        this.perfil.estiloVisual.corDoMarcador = this.corMarcador.value;
        this.perfil.estiloVisual.marcador = this.marcador.value ? this.marcador.value : 'Nenhum';
    }

}
