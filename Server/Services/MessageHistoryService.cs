using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Services;

public class MessageHistoryService(AppDbContext _context) : MessageHistory.MessageHistoryBase
{
   public override async Task<HistoryResponse> GetMessageHistory(HistoryRequest request, ServerCallContext context)
   {
      // Query the database for messages between the two users
      var messages = await _context.Messages
         .Where(m => (m.FromUser == request.FromUser && m.ToUser == request.ToUser) ||
                     (m.FromUser == request.ToUser && m.ToUser == request.FromUser))
         .ToListAsync();
     
      // Map the data to the protobuf Message type
      var response = new HistoryResponse
      {
          Messages =
          {
              messages.Select(m => new Message
              {
                  FromUser = m.FromUser,
                  ToUser = m.ToUser,
                  Content = m.Content
              })
          }
      };

      return response; 
      
   }
}