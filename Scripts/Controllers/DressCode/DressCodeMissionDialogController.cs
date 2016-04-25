using System;
using System.Collections.Generic;

namespace Voltage.Witches.Controllers.DressCode
{
	using Voltage.Witches.Models;
//    using Voltage.Witches.Models.Avatar;
    using Voltage.Witches.Models.Avatar;

	public interface IDressCodeMissionViewModel
	{
		IClothing DressReq { get; }
		bool HasItem { get; }
		bool IsWearingItem { get; }

		DressCodeDialogType Type { get; }
	}


	public class DressCodeMissionDialogViewModel : IDressCodeMissionViewModel
	{
		public IClothing DressReq { get; protected set; }
		public bool HasItem { get; protected set; }
		public bool IsWearingItem { get; protected set; }

		public DressCodeMissionDialogViewModel(IClothing dressReq, Outfit currentOutfit, Inventory currentInventory)
		{
			DressReq = dressReq;
			HasItem = CheckInventoryForItem(currentInventory);
			IsWearingItem = CheckOutfitForItem(currentOutfit);
		}

		bool CheckInventoryForItem(Inventory currentInventory)
		{
			return ((currentInventory.GetCount(DressReq as Item)) > 0);
		}

		bool CheckOutfitForItem(Outfit currentOutfit)
		{
            return (currentOutfit.IsWearingItem(DressReq.Layer_Name));
		}

		public DressCodeDialogType Type
		{
			get
			{
				if(HasItem)
				{
					if(IsWearingItem)
					{
						return DressCodeDialogType.RESUME;
					}
					else
					{
						return DressCodeDialogType.CHANGE;
					}
				}
				else
				{
					return DressCodeDialogType.BUY;
				}

//				return DressCodeDialogType.NONE;
			}
		}

	}

	public enum DressCodeDialogType
	{
		NONE = 0,
		RESUME,		// have item and wearing
		CHANGE,		// have item and not wearing
		BUY			// don't have item
	}

	public enum DressCodeResponse
	{
		CLOSE = 0,
		RESUME = 1,
		CHANGE = 2,
		BUY = 3
	}
}