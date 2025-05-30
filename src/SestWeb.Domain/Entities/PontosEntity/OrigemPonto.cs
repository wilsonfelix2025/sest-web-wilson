namespace SestWeb.Domain.Entities.PontosEntity
{
    public enum OrigemPonto
    {
        Calculado = 0,
        Importado = 1,
        Interpolado = 2,
        Filtrado = 3,
        Montado = 4, //TODO(RCM): Atribuir na Montagem.
        Editado = 5, //TODO(RCM): Atribuir a origem correta nas edições.
        Completado = 6, //TODO(RCM): Atribuir na Completação.
        Interpretado = 7, // //TODO(RCM): Atribuir na criação do perfil interpretado.
        TempoReal = 8 //TODO(RCM): Atribuir na criação durante tempo real..
    }
}