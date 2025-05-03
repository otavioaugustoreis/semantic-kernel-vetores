
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001

// See https://aka.ms/new-console-template for more information
using Elastic.Transport;
using Microsoft.Extensions.VectorData;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Qdrant.Client;
using semantic_kernel_vetores.Entities;


string Key = @"";

var builder = Kernel.CreateBuilder();

//Adicionando serviço de embeddings
builder.AddOpenAITextEmbeddingGeneration(
    modelId: "text-embedding-ada-002", // modelo específico para embeddings
    apiKey: Key
);

Kernel kernel = builder.Build();

IVectorStore vectorStore = new QdrantVectorStore(new QdrantClient("localhost"));


#region COLLECTION
/*
 A collection é como uma tabela no Qdrant.
Define que HotelEntity será a entidade e ulong é o tipo da chave.
*/

#endregion
var collection = vectorStore.GetCollection<ulong, HotelEntity>("skhotels");


//Método de geração de embeddings
async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string textToVectorize)
{
    // Lógica para transforar em vetores

    var embeddingGenerator = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
    var embedding = await embeddingGenerator.GenerateEmbeddingAsync(textToVectorize);
    return embedding;
}

//Criando uma coleção caso ela não exista
//await collection.CreateCollectionIfNotExistsAsync();


//Adicionando a descrição do lugar  e pode ser filtrado.
string descriptionText = "Um lugar onde todos podem ser felizes.";
ulong hotelId = 1;


#region UPSERT
//Upsert = Update ou Insert.
/*
 * Salva/atualiza os hotéis com:

Descrição textual

Embedding vetorial (quando implementado)

Tags e nomes
 */
#endregion
await collection.UpsertAsync(new List<HotelEntity>
{
    new HotelEntity()
    {
        HotelId = hotelId,
        HotelName = "Hotel Feliz",
        Description = descriptionText,
        DescriptionEmbedding = await GenerateEmbeddingAsync(descriptionText),
        Tags = new[] { "Piscina", "luxo" }
    },
    new HotelEntity()
    {
        HotelId = hotelId,
        HotelName = "Hotel Triste",
        Description = descriptionText,
        DescriptionEmbedding = await GenerateEmbeddingAsync(descriptionText),
        Tags = new[] { "Piscina", "padrão" }
    }

});

// Retrieve the upserted record.
//HotelEntity? retrievedHotel = await collection.GetAsync(hotelId);

//Transforma a consulta do usuário em vetor.
ReadOnlyMemory<float> searchVector = await GenerateEmbeddingAsync("Procuro um hotel onde a felicidade do cliente seja a prioridade.");

//Faz a busca semântica no Qdrant.
//Retorna o hotel mais semelhante ao vetor de busca.
var searchResult = collection.SearchEmbeddingAsync(searchVector, top: 1);

//Imprime o hotel mais parecido, com sua pontuação de similaridade.
//Score varia de 0 a 1 (quanto mais próximo de 1, mais parecido).
await foreach (var record in searchResult)
{
    Console.WriteLine("Found hotel description: " + record.Record.Description);
    Console.WriteLine("Found record score: " + record.Score);
}