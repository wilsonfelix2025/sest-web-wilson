import { FileData } from '@utils/interfaces';

export function fileInfoFormatter(fileInfo): FileData {
  let fileData: FileData = {
    profiles: [],
    lithologies: [],
    trajectories: [],
    fixedDeepType: true,
    hasFields: true,
    fields: {},
    filePath: '',
    extras: {
      poçoWeb: { poçoWeb: false },
      wells: {},
    },
  };
  const profiles: { nome: string; tipo: string; unidade: string }[] = fileInfo.perfis;
  const lithologies: string[] = fileInfo.litologias;
  const trajectories: string[] = fileInfo.trajetórias;

  trajectories.forEach((el) => {
    fileData.trajectories.push({
      name: el,
    });
  });

  profiles.forEach((el) => {
    fileData.profiles.push({
      name: el.nome,
      type: el.tipo,
      unit: el.unidade,
    });
  });

  lithologies.forEach((el) => {
    fileData.lithologies.push({
      name: el,
    });
  });

  fileData.fields['Dados Gerais'] = fileInfo.temDadosGerais;
  fileData.fields['Trajetória'] = fileInfo.temTrajetória;
  fileData.fields['Sapatas'] = fileInfo.temSapatas;
  fileData.fields['Objetivos'] = fileInfo.temObjetivos;
  fileData.fields['Estratigrafia'] = fileInfo.temEstratigrafia;
  fileData.fields['Registros'] = fileInfo.temRegistros;
  fileData.fields['Eventos de Perfuração'] = fileInfo.temEventos;
  fileData.extras.wells = fileInfo.poços;

  return fileData;
}
