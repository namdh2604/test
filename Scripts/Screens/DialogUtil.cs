using iGUI;

namespace Voltage.Witches.Screens
{
	public class DialogUtil
	{
		public static int GetMaxLayer(iGUIContainer container)
		{
			int maxLayer = 0;

			if (container.transform.childCount >= 0)
			{
				maxLayer = container.transform.GetChild(0).GetComponent<iGUIElement>().layer;
			}

			for (int i = 1; i < container.transform.childCount; ++i)
			{
				int currentLayer = container.transform.GetChild(i).GetComponent<iGUIElement>().layer;
				if (currentLayer > maxLayer)
				{
					maxLayer = currentLayer;
				}
			}

			return maxLayer;
		}
		
		public static int GetMaxLayerForElement(iGUIElement element)
		{
			int maxLayer = 0;
			
			if (element.transform.childCount >= 0)
			{
				maxLayer = element.transform.GetChild(0).GetComponent<iGUIElement>().layer;
			}
			
			for (int i = 1; i < element.transform.childCount; ++i)
			{
				int currentLayer = element.transform.GetChild(i).GetComponent<iGUIElement>().layer;
				if (currentLayer > maxLayer)
				{
					maxLayer = currentLayer;
				}
			}
			
			return maxLayer;
		}
	}
}

