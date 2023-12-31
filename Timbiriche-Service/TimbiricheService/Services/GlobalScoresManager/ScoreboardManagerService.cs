﻿using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    public partial class UserManagerService : IScoreboardManager
    {
        public List<GlobalScore> GetGlobalScores()
        {
            GlobalScoresManagement dataAccess = new GlobalScoresManagement();
            List<GlobalScore> globalScores = new List<GlobalScore>();

            try
            {
                List<GlobalScores> scores = dataAccess.GetGlobalScores();

                foreach (GlobalScores score in scores)
                {
                    GlobalScore globalScore = new GlobalScore
                    {
                        IdGlobalScore = score.idGlobalScore,
                        IdPlayer = (int)score.idPlayer,
                        WinsNumber = (int)score.winsNumber
                    };

                    globalScores.Add(globalScore);
                }
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }

            return globalScores;
        }

        public int UpdateWins(int idPlayer)
        {
            GlobalScoresManagement dataAccess = new GlobalScoresManagement();
            try
            {
                int response = dataAccess.UpdateWinsPlayer(idPlayer);
                UpdateGlobalScore();

                return response;
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }
    }
}
