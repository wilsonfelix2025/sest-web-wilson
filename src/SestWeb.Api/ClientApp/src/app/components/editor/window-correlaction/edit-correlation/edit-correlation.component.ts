import { Component, ElementRef, ViewChild, OnInit, Inject } from '@angular/core';
import { DialogService } from '@services/dialog.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { LITHO_GROUPS } from '@utils/litho-types-list';
import { ArrayUtils } from '@utils/array';
import { LISTA_MNEMÔNICOS, TIPOS_PERFIS } from '@utils/perfil/tipo-perfil';
import { CorrelationService } from 'app/repositories/correlation.service';
import { Correlation, ObjetoIdentificado } from 'app/repositories/models/correlation';
import { OAuthTokenService } from '@services/oauth.service';
import { Case } from 'app/repositories/models/case';
import { NumberUtils } from '@utils/number';

@Component({
  selector: 'sest-edit-correlation',
  templateUrl: './edit-correlation.component.html',
  styleUrls: ['./edit-correlation.component.scss']
})
export class EditCorrelationComponent implements OnInit {
  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  correlacaoCorrente: Correlation;

  usuarioLogado: string;
  chaveUsuarioLogado: string;

  name: string;
  nameTooltip: string = '';
  description: string;
  descriptionTooltip: string = '';
  author: string;
  authorTooltip = 'Você não é o autor dessa correlação.\nPara conseguir salvar, precisa colocar um nome diferente, assim criando uma nova.';

  equationInvalidTooltip = '';

  BEGIN_REGEX: RegExp = /(?:^|\W)/g;
  END_REGEX: RegExp = /(?:$|\W)/g;

  equation: string;

  especiais = [
    { mnemonico: 'UCS', correlacoes: [], selected: undefined },
    { mnemonico: 'COESA', correlacoes: [], selected: undefined },
    { mnemonico: 'ANGAT', correlacoes: [], selected: undefined },
    { mnemonico: 'RESTR', correlacoes: [], selected: undefined }
  ];

  listaMnemonicos = LISTA_MNEMÔNICOS;
  perfilDeSaida = this.listaMnemonicos[0];
  temPerfilDeSaida = true;
  tooltipExitProfile = 'Perfil de Saída não encontrado na equação.';

  lithoGroups = LITHO_GROUPS;
  lithoGroup: string = this.lithoGroups[0];

  perfilSelecionado;
  profiles: ObjetoIdentificado[] = [];
  PROFILE_REGEX: RegExp = /(\w+)/g;


  lithoGroupsList = LITHO_GROUPS;
  selectedLithoGroup: string;

  constName: string;
  constValue: number;
  consts: ObjetoIdentificado[] = [];

  varName: string;
  varValue: number;
  vars: ObjetoIdentificado[] = [];

  tooltip = {
    'sin': { title: 'Função Seno', tip: 'Seno é uma função trigonométrica. Dado um triângulo retângulo com um de seus ângulos internos igual a θ, define-se sen ⁡ ( θ ) como sendo a razão entre o cateto oposto a θ e a hipotenusa deste triângulo.' },
    'asin': { title: 'Função Arco-Seno', tip: 'Arco-Seno é uma função trigonométrica inversa do Seno.' },
    'sinh': { title: 'Função Seno hiperbólico', tip: 'O Seno hiperbólico é uma função hiperbólica com a propriedade de gerar uma hipérbole.' },
    'asinh': { title: 'Função Arco Seno hiperbólico', tip: 'O Arco Seno hiperbólico é uma função hiperbólica inversa do Seno hiperbólico.' },

    'cos': { title: 'Função Coseno', tip: 'O cosseno (usam-se ainda as formas coseno e co-seno) é uma função trigonométrica. Dado um triângulo retângulo com um de seus ângulos internos igual a θ como sendo a razão entre o cateto adjacente a θ e a hipotenusa deste triângulo.' },
    'acos': { title: 'Função Arco-Coseno', tip: 'Arco-Coseno é uma função trigonométrica inversa do Coseno.' },
    'cosh': { title: 'Função Cosseno hiperbólico', tip: 'O Co-seno hiperbólico é uma função hiperbólica, assim chamadas pois a parametrização de curvas em cosh e senh originam hipérboles, enquanto que as funções trigonométricas dão origem a circunferências.' },
    'acosh': { title: 'Função Arco Cosseno hiperbólico', tip: 'O Arco Cosseno hiperbólico é uma função hiperbólica inversa do Cosseno hiperbólico.' },

    'tan': { title: 'Função Tangente', tip: 'Em trigonometria, tan(θ) (ou tg θ) é a proporção entre o cateto oposto a θ e o cateto adjacente a θ, onde θ é um dos 2 ângulos agudos do triângulo retângulo.' },
    'atan': { title: 'Função Arco-Tangente', tip: 'Arco-Tangente é uma função trigonométrica inversa da Tangente.' },
    'tanh': { title: 'Função Tangente hiperbólica', tip: 'A tangente hiperbólica é uma função hiperbólica. É obtida a partir da razão entre o seno hiperbólico e o cosseno hiperbólico, de forma similar à relação trigonométrica da tangente.' },
    'atanh': { title: 'Função Arco Tangente hiperbólica', tip: 'O Arco Tangente hiperbólica é uma função hiperbólica inversa da Tangente hiperbólica.' },

    'ln': { title: 'Função Logaritmo Natural', tip: 'O logaritmo natural é o logaritmo de base e, onde e é um número irracional aproximadamente igual a 2,718281828459045... chamado de número de Euler. É, portanto, a função inversa da função exponencial.' },
    'log2': { title: 'Função Logaritmo Binário', tip: 'Na matemática, logaritmo binário (log2 n) é o logaritmo de base 2. Consequentemente, é o inverso da potência de dois (2n).' },
    'log': { title: 'Função Logaritmo Comum', tip: 'Logaritmo comum é o logaritmo de base 10.' },
    'exp': { title: 'Função Exponencial', tip: 'A função exponencial é definida por f:R->R através de f(t) = exp(t) = e elevado a t, onde e = 2.7182818284590451.' },
    'a^x': { title: 'Função Exponencial', tip: 'Chama-se função exponencial a função f : R → R + ∗ tal que f(x) = a elevado a x, em que a ∈ R, a > 0, a ≠ 1.' },
    'sqrt': { title: 'Função Raiz Quadrada', tip: 'Na matemática, a raiz quadrada de um número x é um número único e não negativo que, quando multiplicado por si próprio, se iguala a x.' },
    'abs': { title: 'Função Modular', tip: 'O módulo ou valor absoluto (representado matematicamente como |a|) de um número real a é o valor numérico de a desconsiderando seu sinal. Está associado à ideia de distância de um ponto até sua origem (o zero), ou seja, a sua magnitude.' },

    '&&': { title: 'Operador binário E', tip: 'Operador no qual a resposta da operação é verdade se ambas as variáveis de entrada forem verdade.' },
    '||': { title: 'Operador binário OU', tip: 'Operador no qual a resposta da operação é verdade se pelo menos uma das variáveis de entrada for verdade.' },
    '<=': { title: 'Operador relacional Menor ou igual a', tip: 'Operador no qual a resposta da operação é verdade se o operando esquerdo é menor ou igual ao operando direito.' },
    '>=': { title: 'Operador relacional Maior ou igual a', tip: 'Operador no qual a resposta da operação é verdade se o operando esquerdo é maior ou igual ao operando direito.' },
    '!=': { title: 'Operador relacional Diferente a', tip: 'Operador no qual a resposta da operação é verdade se o operando esquerdo não é igual ao operando direito.' },
    '==': { title: 'Operador relacional Igual a', tip: 'Operador no qual a resposta da operação é verdade se o operando esquerdo é igual ao operando direito ' },
    '<': { title: 'Operador relacional Maior que', tip: 'Operador no qual a resposta da operação é verdade o operando esquerdo é maior que o operador direito.' },
    '>': { title: 'Operador relacional Menor que', tip: 'Operador no qual a resposta da operação é verdade se o operando esquerdo é menor que o operando direito.' },
    '?:': { title: 'Conectivo se-então(-senão)', tip: '(condição) ? operação1 : operação2\nEssa expressão executa a operação1 se a (condição) for verdadeira. Caso contrário, executa a operação2.' },

    '_pi': { title: 'PI (π) - constante de Arquimedes', tip: '3.1415926535897931' },
    '_e': { title: 'e - constante de Euler, número natural', tip: '2.7182818284590451' },
  };
  angleFunctions = [
    ['sin', 'asin', 'sinh', 'asinh'],
    ['cos', 'acos', 'cosh', 'acosh'],
    ['tan', 'atan', 'tanh', 'atanh']
  ];

  powFunctions = ['ln', 'log2', 'log', 'exp'];
  othFunctions = ['sqrt', 'abs'];

  logicalOperations = [
    ['(', ')', '&&', '||'],
    ['<=', '>=', '!=', '=='],
    ['<', '>']
  ];

  basicOperations = [
    ['_pi', '_e', '+-', '/'],
    ['7', '8', '9', '*'],
    ['4', '5', '6', '-'],
    ['1', '2', '3', '+'],
    ['0', '.', ',', '=']
  ];

  @ViewChild('equationInput', { static: false }) equationInput: ElementRef;

  matchProfileRegex(regex: RegExp): ObjetoIdentificado[] {
    const value = this.equationInput.nativeElement.value;
    const match: ObjetoIdentificado[] = [];
    const found = ArrayUtils.removeDups(value.match(regex));
    if (found === null) {
      return [];
    }
    found.forEach(word => {
      if (TIPOS_PERFIS[word] !== undefined) {
        match.push({ name: word });
      }
    });
    return match;
  }

  specialProfile(perfilDeSaida: string): boolean {
    let ret = false;
    this.especiais.forEach(el => {
      if (el.mnemonico === perfilDeSaida) {
        ret = true;
        return;
      }
    });
    return ret;
  }

  formatTooltip(el) {
    const tooltip = this.tooltip[el];
    if (tooltip !== undefined) {
      return tooltip.title + '\n' + tooltip.tip;
    }
    return '';
  }


  matchExitProfileRegex(): boolean {
    const regex: RegExp = new RegExp(/\b/g.source + this.perfilDeSaida + /\b\s*=\s*/g.source, 'g');
    const equation = this.equationInput.nativeElement.value;
    const m = regex.exec(equation);
    if (m === null) {
      return false;
    }
    return true;
  }

  constructor(
    public dialogRef: MatDialogRef<EditCorrelationComponent>,
    public dialog: DialogService,
    public correlationService: CorrelationService,
    public auth: OAuthTokenService,
    @Inject(MAT_DIALOG_DATA) public dialogData: any) { }

  ngOnInit() {
    // Seta o caso de estudo com o arquivo recebido.
    this.currCase = this.dialogData.data.case;
    this.getCurrUser();

    if (this.dialogData.data && this.dialogData.data.correlation !== undefined) {
      this.correlationService.getByName(this.currCase.id, this.dialogData.data.correlation.nome).then(res => {
        this.correlacaoCorrente = res;
        this.correlacaoCorrente.origem = this.dialogData.data.correlation.origem;

        this.name = res.nome;
        this.description = res.descrição;
        this.author = res.chaveAutor;
        this.perfilDeSaida = res.perfilSaída;

        this.equationInput.nativeElement.value = res.expressão;
        this.equationInput.nativeElement.focus();
        this.onChangeEquation();
      });
    } else {
      this.author = this.chaveUsuarioLogado;
    }
    this.especiais.forEach(el => {
      el.correlacoes = this.correlationService.list.filter(correlation =>
        correlation.perfilSaída === el.mnemonico);
      el.selected = el.correlacoes[0];
    });
  }

  getCurrUser() {
    this.chaveUsuarioLogado = this.auth.loggedUser !== null ? this.auth.loggedUser.preferred_username : 'Usuario';
    this.usuarioLogado = this.auth.loggedUser !== null ? this.auth.loggedUser.preferred_username : 'Usuario';
  }

  onChangeEquation() {
    this.equationInvalidTooltip = '';
    this.consts = this.correlationService.matchConstVarRegex(this.correlationService.CONST_REGEX, this.equationInput.nativeElement.value);
    this.vars = this.correlationService.matchConstVarRegex(this.correlationService.VAR_REGEX, this.equationInput.nativeElement.value);

    this.verificarConstVarRepetidas();
    this.verificarConstVarValor();
    this.verificarConstVarNome();

    this.profiles = this.matchProfileRegex(this.PROFILE_REGEX);
    this.temPerfilDeSaida = this.matchExitProfileRegex();
  }

  verificarConstVarNome() {
    const nomeInvalido = (el: ObjetoIdentificado, context) => {
      if (TIPOS_PERFIS[el.name] !== undefined) {
        el.valid = false;
        context.equationInvalidTooltip = 'Constantes ou variaveis com nome de perfil.';
      } else if (NumberUtils.isNumber(el.name)) {
        el.valid = false;
        context.equationInvalidTooltip = 'Constantes ou variaveis com nome numérico.';
      } else if (context.lithoGroups.includes(el.name)) {
        el.valid = false;
        context.equationInvalidTooltip = 'Constantes ou variaveis com nome de grupo litologico.';
      } else if (TOKENS_VALIDOS.includes(el.name)) {
        el.valid = false;
        context.equationInvalidTooltip = 'Constantes ou variaveis com nome de Token.';
      }
    };

    this.consts.forEach(el => nomeInvalido(el, this));
    this.vars.forEach(el => nomeInvalido(el, this));
  }

  verificarConstVarValor() {
    const valorInvalido = (el: ObjetoIdentificado, context) => {
      if (!NumberUtils.isNumber(el.value)) {
        el.valid = false;
        context.equationInvalidTooltip = 'Constantes ou variaveis com valor não numérico.';
      }
    };

    this.consts.forEach(el => valorInvalido(el, this));
    this.vars.forEach(el => valorInvalido(el, this));
  }

  verificarConstVarRepetidas() {
    const nomeRepetido = function (el: ObjetoIdentificado, context) {
      if (unique[el.name] === undefined) {
        unique[el.name] = el;
        el.valid = true;
      } else {
        unique[el.name].valid = false;
        el.valid = false;
        context.equationInvalidTooltip = 'Constantes ou variaveis com nomes repetidos.';
      }
    };

    const unique = {};
    this.consts.forEach(el => nomeRepetido(el, this));
    this.vars.forEach(el => nomeRepetido(el, this));
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  addFunction(firstPart, secondPart) {
    const start = this.equationInput.nativeElement.selectionStart;
    const end = this.equationInput.nativeElement.selectionEnd;
    const value = this.equationInput.nativeElement.value;
    const sel = value.substring(start, end);

    const finText = `${value.substring(0, start)}${firstPart}${sel}${secondPart}${value.substring(end)}`;
    this.equationInput.nativeElement.value = finText;
    this.equationInput.nativeElement.focus();
    this.equationInput.nativeElement.selectionEnd = end + firstPart.length;

    this.onChangeEquation();
  }

  addToEquation(value) {
    const startSelection = this.equationInput.nativeElement.selectionStart;
    const currEquation = this.equationInput.nativeElement.value;

    const finText = `${currEquation.substring(0, startSelection)}${value}${currEquation.substring(startSelection)}`;
    this.equationInput.nativeElement.value = finText;
    this.equationInput.nativeElement.focus();
    this.equationInput.nativeElement.selectionEnd = startSelection + value.length;

    this.onChangeEquation();
  }

  addProfileToEquation() {
    this.addToEquation(`${this.perfilSelecionado} `);
    this.perfilSelecionado = undefined;
  }

  addLithoGroupToEquation() {
    this.addToEquation(`(GRUPO_LITOLOGICO == ${this.selectedLithoGroup})? : `);
    this.equationInput.nativeElement.selectionEnd -= 2;
    this.selectedLithoGroup = undefined;
  }

  addConstToEquation() {
    this.addToEquation(`const ${this.constName} = ${this.constValue}, `);
    this.constName = undefined;
    this.constValue = undefined;
  }

  addVarToEquation() {
    this.addToEquation(`var ${this.varName} = ${this.varValue}, `);
    this.varName = undefined;
    this.varValue = undefined;
  }

  addClickedFunction(btn: string) {
    this.addFunction(`${btn}( `, ' ) ');
  }

  addPowFunction() {
    this.addFunction(`a^`, 'x ');
  }

  addIfFunction() {
    this.addFunction(`? `, ': ');
  }

  hasDifferentAuthor(): boolean {
    return this.correlacaoCorrente !== undefined && this.chaveUsuarioLogado !== this.author;
  }

  nameValid(): boolean {
    if (this.correlacaoCorrente !== undefined) {
      if (this.name === this.correlacaoCorrente.nome) {
        if (this.hasDifferentAuthor()) {
          this.nameTooltip = this.authorTooltip;
          return false;
        } else if (this.correlacaoCorrente.origem !== 'Poço') {
          this.nameTooltip = 'Essa correlação já está publicada.\nPara conseguir salvar, precisa colocar um nome diferente, assim criando uma nova.';
          return false;
        }
      }
    }
    this.nameTooltip = '';
    return true;
  }

  canSave(): boolean {
    if (this.equationInvalidTooltip.length > 0) {
      return false;
    }
    if (this.name === undefined || this.name === null || this.name === '') {
      return false;
    }
    if (!this.nameValid()) {
      return false;
    }
    return this.temPerfilDeSaida;
  }

  saveLocal() {
    this.saveCorrelation(false);
  }

  saveGlobal() {
    this.saveCorrelation(true);
  }

  saveCorrelation(global: boolean) {
    if (this.description === '' || this.description === undefined || this.description === null) {
      this.descriptionTooltip = 'Descrição é obrigatorio';
      return;
    }
    const newCorrelation: Correlation = {
      nome: this.name,
      idPoço: this.currCase.id,
      nomeAutor: this.usuarioLogado,
      chaveAutor: this.chaveUsuarioLogado,
      descrição: this.description,
      expressão: this.equationInput.nativeElement.value,
    };

    if (this.correlacaoCorrente !== undefined && this.correlacaoCorrente.chaveAutor === this.chaveUsuarioLogado &&
      this.correlacaoCorrente.origem === 'Poço') {
      this.correlationService.updateLocal(this.currCase.id, newCorrelation).then(res => {
        this.dialogData.data.context.getAllData();
        this.onNoClick();
      });
    } else {
      if (global) {
        this.correlationService.createGlobal(newCorrelation).then(res => {
          this.dialogData.data.context.getAllData();
          this.onNoClick();
        });
      } else {
        this.correlationService.createLocal(newCorrelation).then(res => {
          this.dialogData.data.context.getAllData();
          this.onNoClick();
        });
      }
    }
  }
}

const TOKENS_VALIDOS = [
  'GRUPO_LITOLOGICO',
  'PROFUNDIDADE',
  'PROFUNDIDADE_INICIAL',
  'RHOB_INICIAL',
  'DENSIDADE_AGUA_MAR',
  'LAMINA_DAGUA',
  'MESA_ROTATIVA',
  'ALTURA_ANTEPOCO',
  'CATEGORIA_POCO',
  'OFFSHORE',
  'ONSHORE',
  'STEP'
];
