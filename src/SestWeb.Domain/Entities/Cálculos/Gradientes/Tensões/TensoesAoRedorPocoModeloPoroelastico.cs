using System;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões
{
    public class TensoesAoRedorPocoModeloPoroelastico : ITensoesAoRedorPoco
    {
        private double _tensaoRadial;
        private double _tensaoTangencial;
        private double _tensaoAxial;
        private double _tensaoCisalhanteNoPlanoRadialTangencial;
        private double _tensaoCisalhanteNoPlanoTangencialAxial;
        private double _tensaoCisalhanteNoPlanoRadialAxial;
        private double _pressao;

        public double TensaoRadial { get => _tensaoRadial; private set => _tensaoRadial = value; }
        public double TensaoTangencial { get => _tensaoTangencial; private set => _tensaoTangencial = value; }
        public double TensaoAxial { get => _tensaoAxial; private set => _tensaoAxial = value; }
        public double TensaoCisalhanteNoPlanoRadialTangencial { get => _tensaoCisalhanteNoPlanoRadialTangencial; private set => _tensaoCisalhanteNoPlanoRadialTangencial = value; }
        public double TensaoCisalhanteNoPlanoTangencialAxial { get => _tensaoCisalhanteNoPlanoTangencialAxial; private set => _tensaoCisalhanteNoPlanoTangencialAxial = value; }
        public double TensaoCisalhanteNoPlanoRadialAxial { get => _tensaoCisalhanteNoPlanoRadialAxial; private set => _tensaoCisalhanteNoPlanoRadialAxial = value; }

        public double Pressao { get => _pressao; private set => _pressao = value; }

        private readonly DadosEntradaModeloPoroElastico dadosEntrada;
        private readonly double pw;

        public TensoesAoRedorPocoModeloPoroelastico(double pw, DadosEntradaModeloPoroElastico dadosEntrada, PontoDaMalha pontoMalha)
        {
            this.pw = pw;
            this.dadosEntrada = dadosEntrada;

            var pressao = CalcularPressao(pontoMalha);
            var tensaoRadial = CalcularTensaoRadial(pontoMalha);
            var tensaoTangencial = CalcularTensaoTangencial(pontoMalha);
            var tensaoAxial = CalcularTensaoAxial(-pressao, tensaoRadial, tensaoTangencial);
            var tensaoCisalhanteNoPlanoTangencialAxial = CalcularTensaoCisalhanteNoPlanoTangencialAxial(pontoMalha);
            var tensaoCisalhanteNoPlanoRadialAxial = CalcularTensaoCisalhanteNoPlanoRadialAxial(pontoMalha);
            var tensaoCisalhanteNoPlanoRadialTangencial = CalcularTensaoCisalhanteNoPlanoRadialTangencial(pontoMalha);

            Pressao = pressao;
            TensaoRadial = tensaoRadial;
            TensaoTangencial = tensaoTangencial;
            TensaoAxial = tensaoAxial;
            TensaoCisalhanteNoPlanoTangencialAxial = tensaoCisalhanteNoPlanoTangencialAxial;
            TensaoCisalhanteNoPlanoRadialAxial = tensaoCisalhanteNoPlanoRadialAxial;
            TensaoCisalhanteNoPlanoRadialTangencial = tensaoCisalhanteNoPlanoRadialTangencial;
        }

        private double CalcularTensaoCisalhanteNoPlanoRadialTangencial(PontoDaMalha pontoMalha)
        {
            var young = this.dadosEntrada.Young;
            var poisson = this.dadosEntrada.Poisson;
            var kf = this.dadosEntrada.Kf;
            var ks = this.dadosEntrada.Ks;
            var porosidade = this.dadosEntrada.Porosidade;
            var viscosidade = this.dadosEntrada.Viscosidade;
            var permeabilidade = this.dadosEntrada.Permeabilidade;
            var tempo = this.dadosEntrada.Tempo;

            var pressaoPoros = this.dadosEntrada.PressaoPoros;
            var raioPoco = new RaioPoco(this.dadosEntrada.DiametroPoco);


            var propElasticas = new PropriedadesElasticas(young, poisson, ks, kf, porosidade);
            var propPetrofisicas = new PropriedadesPetrofisicas(porosidade, permeabilidade, viscosidade, propElasticas);

            var cf = propPetrofisicas.Cf;

            var calculador = new CalculadorTensoesCisalhanteRadialTangencial(raioPoco, cf, propElasticas, this.dadosEntrada.MatrizTensorDeTensoes);
            var tensoes = calculador.CalcularTensoesCisalhanteRadialTangencial(tempo, pontoMalha.Raio, pontoMalha.Angulo);
            return tensoes.TensaoCisalhanteRadialTangencialI;
        }

        private double CalcularTensaoCisalhanteNoPlanoRadialAxial(PontoDaMalha pontoMalha)
        {
            var young = this.dadosEntrada.Young;
            var poisson = this.dadosEntrada.Poisson;
            var kf = this.dadosEntrada.Kf;
            var ks = this.dadosEntrada.Ks;
            var porosidade = this.dadosEntrada.Porosidade;
            var viscosidade = this.dadosEntrada.Viscosidade;
            var permeabilidade = this.dadosEntrada.Permeabilidade;
            var tempo = this.dadosEntrada.Tempo;

            var pressaoPoros = this.dadosEntrada.PressaoPoros;
            var raioPoco = new RaioPoco(this.dadosEntrada.DiametroPoco);


            var propElasticas = new PropriedadesElasticas(young, poisson, ks, kf, porosidade);
            var propPetrofisicas = new PropriedadesPetrofisicas(porosidade, permeabilidade, viscosidade, propElasticas);

            var cf = propPetrofisicas.Cf;

            var tensaoCisalhanteRadialAxialModo3 = new TensaoCisalhanteRadialAxialModo3(raioPoco, pontoMalha.Raio, pontoMalha.Angulo, this.dadosEntrada.MatrizTensorDeTensoes);
            return tensaoCisalhanteRadialAxialModo3.Value;
        }

        private double CalcularTensaoCisalhanteNoPlanoTangencialAxial(PontoDaMalha pontoMalha)
        {
            var young = this.dadosEntrada.Young;
            var poisson = this.dadosEntrada.Poisson;
            var kf = this.dadosEntrada.Kf;
            var ks = this.dadosEntrada.Ks;
            var porosidade = this.dadosEntrada.Porosidade;
            var viscosidade = this.dadosEntrada.Viscosidade;
            var permeabilidade = this.dadosEntrada.Permeabilidade;
            var tempo = this.dadosEntrada.Tempo;

            var pressaoPoros = this.dadosEntrada.PressaoPoros;
            var raioPoco = new RaioPoco(this.dadosEntrada.DiametroPoco);


            var propElasticas = new PropriedadesElasticas(young, poisson, ks, kf, porosidade);
            var propPetrofisicas = new PropriedadesPetrofisicas(porosidade, permeabilidade, viscosidade, propElasticas);

            var cf = propPetrofisicas.Cf;

            var tensaoCisalhanteTangencialAxialModo3 = new TensaoCisalhanteTangencialAxialModo3(raioPoco, pontoMalha.Raio, pontoMalha.Angulo, this.dadosEntrada.MatrizTensorDeTensoes);

            return tensaoCisalhanteTangencialAxialModo3.Value;
        }

        private double CalcularTensaoAxial(double pressao, double tensaoRadial, double tensaoTangencial)
        {
            var young = this.dadosEntrada.Young;
            var poisson = this.dadosEntrada.Poisson;
            var kf = this.dadosEntrada.Kf;
            var ks = this.dadosEntrada.Ks;
            var porosidade = this.dadosEntrada.Porosidade;
            var viscosidade = this.dadosEntrada.Viscosidade;
            var permeabilidade = this.dadosEntrada.Permeabilidade;
            var tempo = this.dadosEntrada.Tempo;

            var pressaoPoros = this.dadosEntrada.PressaoPoros;
            var raioPoco = new RaioPoco(this.dadosEntrada.DiametroPoco);


            var propElasticas = new PropriedadesElasticas(young, poisson, ks, kf, porosidade);
            var propPetrofisicas = new PropriedadesPetrofisicas(porosidade, permeabilidade, viscosidade, propElasticas);

            var cf = propPetrofisicas.Cf;

            var tensaoAxialModo1 = new TensaoAxialModo1(pressao, tensaoRadial, tensaoTangencial, propElasticas);
            var tensaoAxialModo2 = new TensaoAxialModo2(pressaoPoros, this.dadosEntrada.MatrizTensorDeTensoes, propElasticas);

            return tensaoAxialModo1.Value + tensaoAxialModo2.Value;
        }

        private double CalcularTensaoTangencial(PontoDaMalha pontoMalha)
        {
            var young = this.dadosEntrada.Young;
            var poisson = this.dadosEntrada.Poisson;
            var kf = this.dadosEntrada.Kf;
            var ks = this.dadosEntrada.Ks;
            var porosidade = this.dadosEntrada.Porosidade;
            var viscosidade = this.dadosEntrada.Viscosidade;
            var permeabilidade = this.dadosEntrada.Permeabilidade;
            var tempo = this.dadosEntrada.Tempo;

            var pressaoPoros = this.dadosEntrada.PressaoPoros;
            var raioPoco = new RaioPoco(this.dadosEntrada.DiametroPoco);


            var propElasticas = new PropriedadesElasticas(young, poisson, ks, kf, porosidade);
            var propPetrofisicas = new PropriedadesPetrofisicas(porosidade, permeabilidade, viscosidade, propElasticas);

            var cf = propPetrofisicas.Cf;

            var calculadorTensoesTangenciais = new CalculadorTensoesTangenciais(raioPoco, pw, cf, propElasticas, this.dadosEntrada.MatrizTensorDeTensoes, pressaoPoros, this.dadosEntrada);
            var tensoesTangenciais = calculadorTensoesTangenciais.CalcularTensoesTangenciais(tempo, pontoMalha);

            return tensoesTangenciais.TensaoTangencialI;

        }

        private double CalcularPressao(PontoDaMalha pontoMalha)
        {
            var young = this.dadosEntrada.Young;
            var poisson = this.dadosEntrada.Poisson;
            var kf = this.dadosEntrada.Kf;
            var ks = this.dadosEntrada.Ks;
            var porosidade = this.dadosEntrada.Porosidade;
            var viscosidade = this.dadosEntrada.Viscosidade;
            var permeabilidade = this.dadosEntrada.Permeabilidade;
            var tempo = this.dadosEntrada.Tempo;

            var pressaoPoros = this.dadosEntrada.PressaoPoros;
            var raioPoco = new RaioPoco(this.dadosEntrada.DiametroPoco);


            var propElasticas = new PropriedadesElasticas(young, poisson, ks, kf, porosidade);
            var propPetrofisicas = new PropriedadesPetrofisicas(porosidade, permeabilidade, viscosidade, propElasticas);

            var cf = propPetrofisicas.Cf;

            var calculadorPressoes = new CalculadorPressoes(pressaoPoros, pw, cf, raioPoco, propElasticas, this.dadosEntrada.MatrizTensorDeTensoes, this.dadosEntrada);

            var pressoes = calculadorPressoes.CalcularPressoes(tempo, pontoMalha);

            return -pressoes.PressaoI;
        }

        private double CalcularTensaoRadial(PontoDaMalha pontoMalha)
        {
            var young = this.dadosEntrada.Young;
            var poisson = this.dadosEntrada.Poisson;
            var kf = this.dadosEntrada.Kf;
            var ks = this.dadosEntrada.Ks;
            var porosidade = this.dadosEntrada.Porosidade;
            var viscosidade = this.dadosEntrada.Viscosidade;
            var permeabilidade = this.dadosEntrada.Permeabilidade;
            var tempo = this.dadosEntrada.Tempo;

            var pressaoPoros = this.dadosEntrada.PressaoPoros;
            var raioPoco = new RaioPoco(this.dadosEntrada.DiametroPoco);


            var propElasticas = new PropriedadesElasticas(young, poisson, kf, ks, porosidade);
            var propPetrofisicas = new PropriedadesPetrofisicas(porosidade, permeabilidade, viscosidade, propElasticas);

            var cf = propPetrofisicas.Cf;

            var calculadorTensaoRadial = new CalculadorTensoesRadiais(raioPoco, propElasticas, this.dadosEntrada.MatrizTensorDeTensoes, pw, pressaoPoros, cf, dadosEntrada);
            var tensoesRadiais = calculadorTensaoRadial.CalcularTensoesRadiais(tempo, pontoMalha);

            return tensoesRadiais.TensaoRadialI;
        }

    }

    public class DadosEntradaModeloPoroElastico
    {
        private double _pressaoPoros;
        private double _tensaoMenor;
        private double _tensaoMaior;
        private double _tensaoVertical;
        private double _pw;
        private double _temperaturaFormacao;
        private double _temperaturaPoco;
        private double _young;
        private double _ks;
        private double _kf;
        private double _permeabilidade;
        private double _viscosidade;
        private TipoSal _tipoSal;
        private double _coefInchamento;
        private double _temperaturaFormacao_FisicoQuimico;

        public double Tempo { get; set; }
        public double Biot { get; set; }
        public double Poisson { get; set; }
        public double DiametroPoco { get; set; }
        public double PressaoPoros { get => _pressaoPoros; set => _pressaoPoros = value; }
        public double Pv { get; set; }
        public double TensaoMenor { get => _tensaoMenor; set => _tensaoMenor = value; }
        public double TensaoMaior { get => _tensaoMaior; set => _tensaoMaior = value; }
        public double Young { get => _young; set => _young = UnitConverter.PsiToPascal(value); }
        public double Ks { get => _ks; set => _ks = UnitConverter.PsiToPascal(value); }
        public double Kf { get => _kf; set => _kf = UnitConverter.PsiToPascal(value); }
        public double Porosidade { get; set; }
        public double Viscosidade { get => _viscosidade; set => _viscosidade = UnitConverter.cPToPa_s(value); }
        public double Permeabilidade { get => _permeabilidade; set => _permeabilidade = UnitConverter.mDToM2(value); }
        public double TensaoVertical { get => _tensaoVertical; set => _tensaoVertical = value; }

        public bool EfeitoTermico { get; set; }
        public double DifusividadeTermica { get; set; }
        public double ExpansaoTermicaRocha { get; set; }
        public double ExpansaoTermicaFluidoPoros { get; set; }
        public double TemperaturaPoco { get => _temperaturaPoco; set => _temperaturaPoco = UnitConverter.CelciusToKelvin(value); }
        public double TemperaturaFormacao { get => _temperaturaFormacao; set => _temperaturaFormacao = UnitConverter.CelciusToKelvin(value); }

        public bool EfeitoFisicoQuimico { get; set; }

        public double CoefReflexao { get; set; }
        public double CoefInchamento { get => _coefInchamento; set => _coefInchamento = UnitConverter.PsiToPascal(value); }
        public double CoefDifusaoSoluto { get; set; }
        public double DensidadeFluidoFormacao { get; set; }
        public double TemperaturaFormacao_FisicoQuimico { get => _temperaturaFormacao_FisicoQuimico; set => _temperaturaFormacao_FisicoQuimico = UnitConverter.CelciusToKelvin(value); }

        public double ConcentracaoSolutoFluidoPerfuracao { get; set; }
        public double ConcentracaoSolutoFluidoRocha { get; set; }

        public TipoSal TipoSal
        {
            get => _tipoSal;
            set
            {
                _tipoSal = value;

                if (_tipoSal == TipoSal.NaCl)
                {
                    CoefDissociacaoSoluto = 1.86400;
                    MassaMolarSoluto = 0.05850;
                }
                else if (_tipoSal == TipoSal.CaCL2)
                {
                    CoefDissociacaoSoluto = 2.60100;
                    MassaMolarSoluto = 0.11100;
                }
                else if (_tipoSal == TipoSal.KCL)
                {
                    CoefDissociacaoSoluto = 1.85400;
                    MassaMolarSoluto = 0.07450;
                }
                else
                {
                    throw new Exception($"Tipo de sao não reconhecido: {_tipoSal}");
                }
            }
        }

        public double CoefDissociacaoSoluto { get; private set; }
        public double MassaMolarSoluto { get; private set; }


        public double Pw { get => _pw; set => _pw = UnitConverter.PsiToPascal(value); }

        public MatrizTensorDeTensoes MatrizTensorDeTensoes { get; set; }


    }

    public enum TipoSal
    {
        NaCl,
        CaCL2,
        KCL
    }
}
