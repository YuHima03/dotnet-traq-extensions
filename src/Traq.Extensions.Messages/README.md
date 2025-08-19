# Traq.Extensions.Messages

Provides extension methods for handling messages in the traQ service.

## Features

### Message analyzation

You can easily extract embed contents from messages using the `MessageElementEnumerator` type.
This allows you to iterate through the elements of a message and extract information such as url embeds, user mentions, channel reference, and message citation.

```cs
using var enumerator = new MessageElementEnumerator(message);
foreach (var elem in enumerator)
{
    if (elem.Kind == MessageElementKind.Embedding)
   {
        var embed = e.GetEmbedding();
        if (embed.Type == EmbeddingType.UserMention)
        {
            var uid = embedded.Id;
            // Handle user mention
        }
    }
}
```
