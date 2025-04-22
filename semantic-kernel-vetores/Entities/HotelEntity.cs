using Microsoft.Extensions.VectorData;

namespace semantic_kernel_vetores.Entities
{
    public class HotelEntity
    {
        [VectorStoreRecordKey]
        public ulong HotelId { get; set; }

        [VectorStoreRecordData(IsFilterable = true)]
        public string HotelName { get; set; }

        [VectorStoreRecordData(IsFullTextSearchable = true)]
        public string Description { get; set; }

        [VectorStoreRecordVector(Dimensions: 4, DistanceFunction.CosineSimilarity, IndexKind.Hnsw)]
        public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

        [VectorStoreRecordData(IsFilterable = true)]
        public string[] Tags { get; set; }
    }
}
