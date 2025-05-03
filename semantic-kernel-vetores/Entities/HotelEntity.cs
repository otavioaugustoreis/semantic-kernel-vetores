using Microsoft.Extensions.VectorData;

namespace semantic_kernel_vetores.Entities
{
    #region COMO_FUNCIONA
    /*
        Cenário de Busca Semântica
        Usuário busca: "hotel aconchegante para lua de mel".

        Sistema:

        Converte a consulta em um vetor (embedding).

        Compara com DescriptionEmbedding dos hoteis usando similaridade de cosseno.

        Retorna os mais relevantes, mesmo sem correspondência exata de palavras.

        Vantagens
        Encontra relações conceituais (ex.: "aconchegante" → "romântico", "intimista").

        Combina buscas vetoriais com filtros tradicionais (ex.: tags "luxo" + semântica "vista para o mar").
     */
    #endregion
    public class HotelEntity
    {
        //Chave primária
        [VectorStoreRecordKey]
        public ulong HotelId { get; set; }

        //Permite filtrar por esse campo (ex.: WHERE HotelName = "Copacabana Palace").
        [VectorStoreRecordData(IsIndexed = true)]
        public string HotelName { get; set; }

        //Habilita busca por texto completo (ex.: encontrar "hotel à beira-mar" mesmo se a descrição usar "frente para o oceano").
        [VectorStoreRecordData(IsIndexed = true)]
        public string Description { get; set; }

        //O Coração Vetorial
        //Armazena o embedding (vetor numérico) da descrição do hotel.
        /*
         Parâmetros:
          Dimensions: 4: Tamanho do vetor (4 é apenas ilustrativo - na prática, use 768, 1024, etc.).
          DistanceFunction.CosineSimilarity: Métrica para comparar vetores (outras opções: Euclidean, DotProduct).
          IndexKind.Hnsw: Algoritmo de indexação para buscas eficientes (Hierarchical Navigable Small World).
        */
        [VectorStoreRecordVector(Dimensions : 4, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Flat)]
        public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

        //Útil para filtros múltiplos (ex.: Tags.Contains("luxo")).
        [VectorStoreRecordData(IsIndexed = true)]
        public string[] Tags { get; set; }
    }
}
