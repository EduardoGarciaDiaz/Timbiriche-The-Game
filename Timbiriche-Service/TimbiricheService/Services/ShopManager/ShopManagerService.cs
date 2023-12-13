﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    public partial class UserManagerService : IShopManager
    {
        public bool BuyColor(ShopColor color, int idPlayer)
        {
            try
            {
                bool purchaseCompleted = ShopManagement.BuyColor(color.IdColor, idPlayer);

                if (purchaseCompleted)
                {
                    purchaseCompleted = ShopManagement.SubstractPlayerCoins(idPlayer, color.ColorCost);
                }

                return purchaseCompleted;
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public bool BuyStyle(ShopStyle style, int idPlayer)
        {
            try
            {
                bool purchaseCompleted = ShopManagement.BuyStyle(style.IdStyle, idPlayer);

                if (purchaseCompleted)
                {
                    purchaseCompleted = ShopManagement.SubstractPlayerCoins(idPlayer, style.StyleCost);
                }

                return purchaseCompleted;
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public List<ShopColor> GetColors(int idPlayer)
        {
            try
            {
                List<Colors> colorsFromDataBase = ShopManagement.GetColors();
                List<PlayerColors> playerColors = ShopManagement.GetPlayerColors(idPlayer);
                List<ShopColor> colors = new List<ShopColor>();

                foreach (Colors color in colorsFromDataBase)
                {
                    bool ownedColor = playerColors.Exists(pc => pc.idColor == color.idColor);

                    ShopColor singleColor = new ShopColor();
                    singleColor.IdColor = color.idColor;
                    singleColor.ColorName = color.colorName;
                    singleColor.ColorCost = (int)color.cost;
                    singleColor.HexadecimalCode = color.hexadecimalCode;
                    singleColor.OwnedColor = ownedColor;

                    colors.Add(singleColor);
                }

                return colors;
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public List<ShopStyle> GetStyles(int idPlayer)
        {
            try
            {
                List<Styles> stylesFromDataBase = ShopManagement.GetStyles();
                List<PlayerStyles> playerStyles = ShopManagement.GetPlayerStyles(idPlayer);
                List<ShopStyle> styles = new List<ShopStyle>();

                foreach (Styles style in stylesFromDataBase)
                {
                    bool ownedStyle = playerStyles.Exists(ps => ps.idStyle == style.idStyle);

                    ShopStyle singleStyle = new ShopStyle();
                    singleStyle.IdStyle = style.idStyle;
                    singleStyle.StyleName = style.styleName;
                    singleStyle.StyleCost = (int)style.cost;
                    singleStyle.StylePath = style.path;
                    singleStyle.OwnedStyle = ownedStyle;

                    styles.Add(singleStyle);
                }

                return styles;
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }
    }
}