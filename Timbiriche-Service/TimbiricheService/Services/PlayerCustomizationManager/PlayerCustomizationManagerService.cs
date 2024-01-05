using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    public partial class UserManagerService : IPlayerCustomizationManager
    {
        public List<PlayerColor> GetMyColors(int idPlayer)
        {
            List<PlayerColor> myColors = new List<PlayerColor>();
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();

            try
            {
                List<PlayerColors> playerColorsDataAccess = dataAccess.GetMyColorsByIdPlayer(idPlayer);

                foreach (PlayerColors playerColor in playerColorsDataAccess)
                {
                    PlayerColor playerColorAuxiliar = new PlayerColor();
                    playerColorAuxiliar.IdPlayerColors = playerColor.idPlayerColors;
                    playerColorAuxiliar.IdPlayer = (int)playerColor.idPlayer;
                    playerColorAuxiliar.IdColor = (int)playerColor.idColor;

                    myColors.Add(playerColorAuxiliar);
                }

                return myColors;
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

        public string GetHexadecimalColors(int idColor)
        {
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();

            try
            {
                return dataAccess.GetHexadecimalColorByIdColor(idColor);
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

        public int SelectMyColor(int idPlayer, int idColor)
        {
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();

            try
            {
                return dataAccess.UpdateMyColorSelected(idPlayer, idColor);
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

        public bool CheckColorForPlayer(int idPlayer, int idColor)
        {
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();

            try
            {
                return dataAccess.SearchInMyColors(idPlayer, idColor);
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

        public List<PlayerStyle> GetMyStyles(int idPlayer)
        {
            List<PlayerStyle> myStyles = new List<PlayerStyle>();
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();

            try
            {
                List<PlayerStyles> playerStylesDataAccess = dataAccess.GetMyStylesByIdPlayer(idPlayer);

                foreach (PlayerStyles playerStyle in playerStylesDataAccess)
                {
                    PlayerStyle playerStyleAxiliar = new PlayerStyle();
                    playerStyleAxiliar.IdPlayerStyle = playerStyle.idPlayerStyles;
                    playerStyleAxiliar.IdPlayer = (int)playerStyle.idPlayer;
                    playerStyleAxiliar.IdStyle = (int)playerStyle.idStyle;

                    myStyles.Add(playerStyleAxiliar);
                }

                return myStyles;
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

        public string GetStylePath(int idStyle)
        {
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();

            try
            {
                return dataAccess.GetStylePathByIdStyle(idStyle);
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

        public int SelectMyStyle(int idPlayer, int idStyle)
        {
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();

            try
            {
                return dataAccess.UpdateMyStyleSelected(idPlayer, idStyle);

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
