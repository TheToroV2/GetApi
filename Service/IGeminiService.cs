using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IGeminiService
    {
        Task<string> GetChatResponse(string prompt);
        Task<string> TranslateToSQL(string naturalLanguageQuery);
    }
}
