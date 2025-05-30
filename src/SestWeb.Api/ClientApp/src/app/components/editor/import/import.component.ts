import {
  Component,
  Inject,
  OnInit,
  Type,
  ViewChild,
  ComponentFactoryResolver,
  ViewContainerRef,
  ComponentRef,
} from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FileData, ImportData } from '@utils/interfaces';
import { Class } from 'highcharts';
import { Case } from 'app/repositories/models/case';
import { DatasetService } from '@services/dataset/dataset.service';
import { ImportService } from 'app/repositories/import.service';

@Component({
  selector: 'app-import',
  templateUrl: './import.component.html',
  styleUrls: ['./import.component.scss'],
})
export class ImportComponent implements OnInit {
  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  /**
   * A div para adicionar o componente atual do diálogo.
   */
  @ViewChild('dialogInfo', { read: ViewContainerRef, static: true }) dialogInfo: ViewContainerRef;

  /**
   * Lista contendo os componentes dos passos anteriores da importação.
   *
   * Do mais antigo para o mais recente.
   */
  previousComponent: Class<any>[] = [];

  /**
   * O componente do passo atual da importação.
   */
  currentComponent: Class<any>;

  /**
   * O componente do proximo passo da importação.
   */
  nextComponent: Class<any>;

  /**
   * O componente exibido atualmente.
   */
  infoComponent: ComponentRef<any>;

  /**
   * Observador que se inscreve no evento generico de output do componente exibido atualmente.
   *
   * Esse evento de output deve retornar um único objeto.
   */
  $outputEvent;

  /**
   * Se foi preenchido todo o necessário para avançar para o próximo passo.
   */
  completedStep = false;

  /**
   * Dados que devem ser passados para o próximo passo.
   */
  nextStepData: FileData;

  /**
   * Dados que devem ser passados para o componente anterior.
   */
  previousStepData: FileData;

  /**
   * Dados que foram escolhidos para serem importados.
   */
  dataToImport: ImportData;

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<ImportComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: { initialComponent: Class<any>; initialData?} },
    private componentFactoryResolver: ComponentFactoryResolver,
    public importService: ImportService,

    private dataset: DatasetService,
  ) { }

  ngOnInit() {
    // Seta o caso de estudo com o arquivo recebido.
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
    // Seta o componente atual como o componente inicial recebido.
    this.currentComponent = this.data.data.initialComponent;
    // Adiciona o componente atual dinamicamente.
    this.addDynamicComponent(this.currentComponent, { otherData: this.data.data.initialData });
  }

  /**
   * Adiciona dinamicamente um componente no dialogo.
   *
   * @param component componente a ser adicionado no dialogo
   * @param inputData dados para serem adicionados ao componente
   */
  public addDynamicComponent(component: Type<any>, inputData: { fileData?: FileData; otherData?}) {
    const comp = this.componentFactoryResolver.resolveComponentFactory(component);
    // Cria o componente na div e salva a referencia na variavel.
    this.infoComponent = this.dialogInfo.createComponent(comp);
    // Fornece os dados ao componente criado.
    this.infoComponent.instance.data = inputData;

    this.completedStep = false;
    this.previousStepData = inputData.fileData;
    this.nextComponent = undefined;
    this.nextStepData = undefined;
    // Inscreve observador para ouvir o Output.
    this.$outputEvent = this.infoComponent.instance.someEvent.subscribe((outputData) => {
      this.completedStep = outputData.cangoNext;
      if (this.completedStep && outputData.type) {
        this.nextComponent = outputData.type.page;
        this.nextStepData = outputData.data;
      }
    });
  }

  /**
   * Remover componente atual.
   */
  public removeDynamicComponent() {
    this.$outputEvent.unsubscribe();
    this.infoComponent.destroy();
  }

  /**
   * Fechar dialogo.
   */
  onNoClick(): void {
    this.dialogRef.close();
  }

  /**
   * Voltar para o passo anterior da importação.
   */
  previousStep() {
    // Pega o componente anterior mais recente e transforma no componente atual.
    this.currentComponent = this.previousComponent.pop();

    this.removeDynamicComponent();
    this.addDynamicComponent(this.currentComponent, { fileData: this.previousStepData });
  }

  /**
   * Avançar para o próximo passo da importação.
   */
  nextStep() {
    // Adiciona o componente atual como o componente anterior mais recente.
    this.previousComponent.push(this.currentComponent);
    // Transforma o proximo componente no componente atual.
    this.currentComponent = this.nextComponent;

    this.removeDynamicComponent();
    this.addDynamicComponent(this.currentComponent, { fileData: this.nextStepData });
  }

  /**
   * Importar dados da tela atual
   */
  submit() {
    this.loading = true;
    // Se a tela tiver função para exportar os dados
    if (this.infoComponent.instance.exportData) {
      this.dataToImport = this.infoComponent.instance.exportData();
      if (this.dataToImport !== undefined) {
        this.importService.importFile(this.dataToImport).then((response) => {
          this.loading = false;
          location.reload();
        }).catch(() => { this.loading = false; });
      } else {
        this.loading = false;
      }
    } else if (this.infoComponent.instance.importData) {
      this.infoComponent.instance.importData();
      this.loading = false;
      this.onNoClick();
    } else {
      this.loading = false;
    }
  }
}
