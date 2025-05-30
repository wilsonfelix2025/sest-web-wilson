using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Poço.Objetivos;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using SestWeb.Domain.Factories;
using SestWeb.Domain.Helpers;
using SestWeb.Domain.Importadores.Deep.Sest5;

namespace SestWeb.Infra.MongoDataAccess.Seed
{
    public class Seeder
    {
        public async Task<string> SeedAsync(Context context)
        {
            var ids = "";
            var locations = GetSeedFileLocation();
            foreach (var location in locations)
            {
                var leitorSest5 = new LeituraArquivoSest5(location);

                if (!leitorSest5.TryObterTrajetória(out var trajetória) ||
                    !leitorSest5.TryObterLitologias(out var litologias) ||
                    !leitorSest5.TryObterDadosGerais(out var dadosGerais) ||
                    !leitorSest5.TryObterPerfis(out var perfis, trajetória, litologias?.Single(x => x.Classificação == TipoLitologia.Adaptada)) ||
                    !leitorSest5.TryObterSapatas(out var sapatas) ||
                    !leitorSest5.TryObterObjetivos(out var objetivos) ||
                    !leitorSest5.TryObterEstratigrafia(trajetória, out var estratigrafia))
                {
                    throw new InvalidOperationException("Não conseguiu ler os dados do SEST 5.");
                }
                string id = "0";
                Well well = await context.Wells.Find(w => w.Id == "1").Limit(1).FirstOrDefaultAsync();
                well.Files.Add(id);
                await context.Wells.ReplaceOneAsync(w => w.Id == well.Id, well);
                File newFile = new File($"https://pocoweb.petro.intelie.net/api/public/file/{id}/?rev=8159", dadosGerais.Identificação.Nome, well.Url, "sesttr.retroanalysis");
                var poço = PoçoFactory.CriarPoço(id, dadosGerais.Identificação.NomePoço, dadosGerais.Identificação.TipoPoço);

                poço.DadosGerais = dadosGerais;
                poço.Trajetória = trajetória;
                poço.Litologias = litologias;
                poço.Estratigrafia = estratigrafia;

                var sapatasFactory = poço.ObterSapataFactory();
                foreach (var sapataDto in sapatas)
                {
                    var sapata = sapatasFactory.CriarSapata(sapataDto.Pm.ToDouble(), sapataDto.Diâmetro);
                    poço.Sapatas.Add(sapata);
                }

                var objetivoFactory = poço.ObterObjetivoFactory();
                foreach (var objetivoDto in objetivos)
                {
                    var objetivo = objetivoFactory.CriarObjetivo(objetivoDto.Pm.ToDouble(), Enum.Parse<TipoObjetivo>(objetivoDto.TipoObjetivo));
                    poço.Objetivos.Add(objetivo);
                }

                await context.Files.InsertOneAsync(newFile);
                ids += poço.Id + " / ";
                perfis.ForEach(p => p.IdPoço = poço.Id);
                await context.Perfis.InsertManyAsync(perfis);
                var perfisList = await context.Perfis.Find(_ => true).ToListAsync();
                perfisList.ForEach(p =>
                {
                    poço.State.Tree.ForEach(t =>
                    {
                        if (t.Name == p.GrupoPerfis.Nome)
                        {
                            var perfil = new Tree {Id = p.Id.ToString(), Fixa = false, Name = p.Nome, Tipo = "0"};
                            t.Data.Add(perfil);
                        }
                    });
                });
                await context.Poços.InsertOneAsync(poço);
                break;
            }
            return ids;

        }

        private List<string> GetSeedFileLocation()
        {
            var location = GetType().Assembly.CodeBase;
            var i = location.LastIndexOf('/');

            location = location.Remove(i);
            var location1 = location + "/MongoDataAccess/Seed/Jo_Editado.xml";
            var location2 = location + "/MongoDataAccess/Seed/3-RO-4-RJS_Ex1.xml";

            var lista = new List<string> { location1, location2 };

            return lista;
        }
    }
}