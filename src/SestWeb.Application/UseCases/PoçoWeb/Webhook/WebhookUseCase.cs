using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using SestWeb.Domain.Webhook;

namespace SestWeb.Application.UseCases.PoçoWeb.Webhook
{
    internal class WebhookUseCase : IWebhookUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IOpUnitReadOnlyRepository _opUnitReadOnlyRepository;
        private readonly IOilFieldReadOnlyRepository _oilFieldReadOnlyRepository;
        private readonly IWellReadOnlyRepository _wellReadOnlyRepository;
        private readonly IFileReadOnlyRepository _fileReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IWellWriteOnlyRepository _wellWriteOnlyRepository;
        private readonly IOilFieldWriteOnlyRepository _oilFieldWriteOnlyRepository;
        private readonly IOpUnitWriteOnlyRepository _opUnitWriteOnlyRepository;

        public WebhookUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IOpUnitReadOnlyRepository opUnitReadOnlyRepository,
            IOilFieldReadOnlyRepository oilFieldReadOnlyRepository, IWellReadOnlyRepository wellReadOnlyRepository,
            IFileReadOnlyRepository fileReadOnlyRepository,
            IPoçoWriteOnlyRepository poçoWriteOnlyRepository, IWellWriteOnlyRepository wellWriteOnlyRepository,
            IOilFieldWriteOnlyRepository oilFieldWriteOnlyRepository, IOpUnitWriteOnlyRepository opUnitWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _opUnitReadOnlyRepository = opUnitReadOnlyRepository;
            _oilFieldReadOnlyRepository = oilFieldReadOnlyRepository;
            _wellReadOnlyRepository = wellReadOnlyRepository;
            _fileReadOnlyRepository = fileReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _wellWriteOnlyRepository = wellWriteOnlyRepository;
            _oilFieldWriteOnlyRepository = oilFieldWriteOnlyRepository;
            _opUnitWriteOnlyRepository = opUnitWriteOnlyRepository;
        }

        public async Task<WebhookOutput> Execute(string json)
        {
            try
            {
                var data = JObject.Parse(json);

                if (data == null)
                    throw new Exception("json não reconhecido.");

                var eventTypeToken = data.SelectToken("event_type");

                if (eventTypeToken != null)
                {
                    var eventType = eventTypeToken.Value<string>();

                    if (eventType == "logout")
                    {
                        HandleLogout(json);
                    }
                    else if (eventType == "hierarchy_sync")
                    {
                        await HandleHierarchySync(json);
                    }
                    else if (eventType == "file_revision_created")
                    {
                        await HandleFileRevisionCreated(json);
                    }
                    else
                        throw new Exception("event_type não reconhecido.");
                }
                else
                    throw new Exception("event_type não preenchido.");

                // schema

                //{
                //    "$schema": "http://json-schema.org/draft-04/schema#",
                //    "id": "http://jsonschema.net#",
                //    "type": "object",
                //    "additionalProperties": false,
                //    "properties": {
                //        "event_data": {
                //            "type": "object",
                //            "additionalProperties": true
                //        },
                //        "event_type": {
                //            "type": "string"
                //        },
                //        "created_at": {
                //            "type": "string"
                //        }
                //    },
                //    "required": ['event_type', 'event_data', 'created_at']
                //}


                // Logout

                //Content - Type: application / json

                //{
                //    "event_data": { "username": "pedro"},
                //    "event_type": "logout",
                //    "created_at": "2014-07-29T19:49:39Z"
                //}



                // Adicionar elementos de hierarquia

                //Content - Type: application / json

                //{
                //    "event_data": { "add": ["OpUnit 1", "Campo 1", "Poço 1"]},
                //    "event_type": "hierarchy_sync",
                //    "created_at": "2014-07-29T19:49:39Z"
                //}


                //Mover elementos de hierarquia

                // Apenas último item diferente, renomear item.

                //Content - Type: application / json

                //{
                //    "event_data": {
                //        "move": [
                //        ["OpUnit 1", "Campo 1", "Poço 1"],
                //        ["OpUnit 1", "Campo 1", "Poço 1 renomeado"]
                //    },
                //    "event_type": "hierarchy_sync",
                //    "created_at": "2014-07-29T19:49:39Z"
                //}


                //Item do meio diferente, mover item de pai.
                //Content - Type: application / json

                //{
                //    "event_data": {
                //        "move": [
                //        ["OpUnit 1", "Campo 1", "Poço 1"],
                //        ["OpUnit 1", "Campo 2", "Poço 1"]
                //    },
                //    "event_type": "hierarchy_sync",
                //    "created_at": "2014-07-29T19:49:39Z"
                //}



                //Dois últimos itens diferentes, mover e renomear
                //Content - Type: application / json

                //{
                //    "event_data": {
                //        "move": [
                //        ["OpUnit 1", "Campo 1", "Poço 1"],
                //        ["OpUnit 1", "Campo 2", "Poço 1 Renomeado"]
                //    },
                //    "event_type": "hierarchy_sync",
                //    "created_at": "2014-07-29T19:49:39Z"
                //}
                

                return WebhookOutput.Ok();
            }
            catch (Exception ex)
            {
                return WebhookOutput.NotOk(ex.Message);
            }
        }

        private void HandleLogout(string json)
        {
            throw new NotImplementedException();
        }

        // Observar a necessidade de lidar com conflitos de entidades quando estiver movendo/renomeando, especialmente de entidades filhas.
        private async Task HandleFileRevisionCreated(string json)
        {
            throw new NotImplementedException();
        }

        private async Task HandleHierarchySync(string json)
        {
            var data = JObject.Parse(json);
            var eventDataToken = data.SelectToken("event_data");

            if (eventDataToken != null && eventDataToken.HasValues)
            {
                var operation = eventDataToken.First.Path.Split(".").Last();

                if (operation == "add")
                {
                    var property = JsonConvert.DeserializeObject<AddWebhookProperty>(json, new AddWebhookConverter());
                    var result = await AddOperation(property);

                    if (!string.IsNullOrEmpty(result))
                        throw new Exception(result);
                }
                else if (operation == "move")
                {
                    var property = JsonConvert.DeserializeObject<MoveWebhookProperty>(json, new MoveWebhookConverter());
                    await HandleMoveOperation(property);
                }
            }
        }

        private async Task HandleMoveOperation(MoveWebhookProperty property)
        {
            var nomeOpUnitAnterior = property.Data[0, 0].Trim();
            var nomeOilfieldAnterior = property.Data[0, 1].Trim();
            var nomePoçoAnterior = property.Data[0, 2].Trim();
            var nomeOpUnitPosterior = property.Data[1, 0].Trim();
            var nomeOilfieldPosterior = property.Data[1, 1].Trim();
            var nomePoçoPosterior = property.Data[1, 2].Trim();

            var optUnitAnterior = await _opUnitReadOnlyRepository.GetOpUnitByName(nomeOpUnitAnterior);
            var oilFieldAnterior = await _oilFieldReadOnlyRepository.GetOilFieldByName(nomeOilfieldAnterior, optUnitAnterior.Id);
            var well = await _wellReadOnlyRepository.GetWellByName(nomePoçoAnterior, oilFieldAnterior.Id);
            var files = await _fileReadOnlyRepository.GetFilesByWell(well.Id);

            OpUnit optUnit = optUnitAnterior;
            OilField oilField = oilFieldAnterior;

            if (!nomePoçoAnterior.Equals(nomePoçoPosterior))
            {
                if (files != null)
                {
                    //foreach (var file in files)
                    //{
                    //    var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(file.Id);
                    //    if (poço == null)
                    //        continue;

                    //    poço.DadosGerais.Identificação.Nome = nomePoçoPosterior;
                    //    await _poçoWriteOnlyRepository.AtualizarDadosGerais(poço.Id, poço, false);
                    //}
                }

                well.Name = nomePoçoPosterior;
                await _wellWriteOnlyRepository.UpdateWell(well);
            }

            if (!nomeOilfieldAnterior.Equals(nomeOilfieldPosterior))
            {
                if (!nomeOpUnitAnterior.Equals(nomeOpUnitPosterior))
                {
                    optUnit = await _opUnitReadOnlyRepository.GetOpUnitByName(nomeOpUnitPosterior);
                }

                oilField = await _oilFieldReadOnlyRepository.GetOilFieldByName(nomeOilfieldPosterior, optUnit.Id);

                // oilfield renomeado
                if (oilField == null)
                {
                    oilFieldAnterior.Name = nomeOilfieldPosterior;
                }
                else
                {
                    if (oilFieldAnterior.Wells.Contains(well.Id))
                        oilFieldAnterior.Wells.Remove(well.Id);

                    if (!oilField.Wells.Contains(well.Id))
                    {
                        oilField.Wells.Add(well.Id);
                        await _oilFieldWriteOnlyRepository.UpdateOilField(oilField);
                    }

                    well.OilFieldId = oilField.Id;
                    await _wellWriteOnlyRepository.UpdateWell(well);
                }

                await _oilFieldWriteOnlyRepository.UpdateOilField(oilFieldAnterior);
            }

            if (!nomeOpUnitAnterior.Equals(nomeOpUnitPosterior))
            {
                optUnit = await _opUnitReadOnlyRepository.GetOpUnitByName(nomeOpUnitPosterior);

                // optUnit renomeado
                if (optUnit == null)
                {
                    optUnitAnterior.Name = nomeOpUnitPosterior;
                }
                else
                {
                    if (optUnitAnterior.OilFields.Contains(oilFieldAnterior.Id))
                        optUnitAnterior.OilFields.Remove(oilFieldAnterior.Id);

                    if (!optUnit.OilFields.Contains(oilField.Id))
                    {
                        optUnit.OilFields.Add(oilField.Id);
                        await _opUnitWriteOnlyRepository.UpdateOpUnit(optUnit);
                    }

                    oilField.OpUnitId = optUnit.Id;
                    await _oilFieldWriteOnlyRepository.UpdateOilField(oilField);
                }

                await _opUnitWriteOnlyRepository.UpdateOpUnit(optUnitAnterior);
            }
        }

        private async Task<string> AddOperation(AddWebhookProperty property)
        {
            var data = property.Data;
            var qty = data.Count;
            try
            {
                var opUnit = await AddOpUnit(data);
                OilField oilField = null;
                
                if (qty > 1 && opUnit != null)
                {
                    oilField = await AddOilField(data, opUnit.Id);
                    if (!opUnit.OilFields.Contains(oilField.Id))
                    {
                        opUnit.OilFields.Add(oilField.Id);
                        await _opUnitWriteOnlyRepository.UpdateOpUnit(opUnit);
                    }
                }

                if (qty == 3 && oilField != null)
                {
                    var well = await AddWell(data, oilField.Id);
                    if (well != null && !oilField.Wells.Contains(well.Id))
                    {
                        oilField.Wells.Add(well.Id);
                        await _oilFieldWriteOnlyRepository.UpdateOilField(oilField);
                    }
                }
                
                return string.Empty;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }

        private async Task<Well> AddWell(List<string> data, string oilFieldId)
        {
            var well = data[2];
            var hasWell = await _wellReadOnlyRepository.HasWellWithTheSameName(well);

            if (hasWell == false)
            {
                var newWell = new WellRequest(well, oilFieldId);
                return await _wellWriteOnlyRepository.CreateWell(newWell, string.Empty);
            }

            return null;
        }

        private async Task<OpUnit> AddOpUnit(List<string> data)
        {
            var opUnit = data[0];
            var hasOpUnit = await _opUnitReadOnlyRepository.HasOpUnitWithTheSameName(opUnit);

            if (hasOpUnit == false)
            {
                var lastId = int.Parse(_opUnitReadOnlyRepository.GetLastId()) + 1;
                var url = "https://pocoweb.petro.intelie.net/api/public/opunit/" + lastId.ToString() + "/";
                var newOpUnit = new OpUnit(url, opUnit);
                await _opUnitWriteOnlyRepository.CreateOpUnit(newOpUnit);
                return newOpUnit;
            }

            return await _opUnitReadOnlyRepository.GetOpUnitByName(opUnit);
        }

        private async Task<OilField> AddOilField(List<string> data, string opUnitId)
        {
            var oilField = data[1];
            var hasOilField = await _oilFieldReadOnlyRepository.HasOilFieldWithTheSameName(oilField);

            if (hasOilField == false)
            {
                var lastId = int.Parse(_oilFieldReadOnlyRepository.GetLastId()) + 1;
                var url = "https://pocoweb.petro.intelie.net/api/public/oilfield/" + lastId.ToString() + "/";
                var newOilField = new OilField(url, oilField, opUnitId);
                await _oilFieldWriteOnlyRepository.CreateOilField(newOilField);
                return newOilField;
            }

            return await _oilFieldReadOnlyRepository.GetOilFieldByName(oilField, opUnitId);
        }
    }
}
