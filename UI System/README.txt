The flood menu can be used to create dynamic UI such as inventories, stores and more by automatically filing menus with content.

A sample of how I implemented this system can be found in the SAMPLES folder.


COMPONENTS:
UIComponent : MonoBehaviour
	Wrapper class for all types of Menu, useful for storing references in code or in the inspector.

Menu<TCell, TData> : UIComponent
	Inherit from this class to create a menu. The derived class should be added as a component on a UI element.

MenuCell<TData> : MonoBehaviour
	Inherit from this class to create a new cell for a menu to use.
	The cell is responsible for updating UI elements inside of it, such as text or images. 

MenuCellData
	Inherit from this class to create data that cells can use to update their UI elements.


HOW TO:
	1. Create a new script.
	2. Inherit from MenuCellData.
	3. Add fields for any data that needs to be displayed on the UI.

	4. Create a new script.
	5. Inherit from MenuCell<TData>.
	6. Create a UI prefab with this component.
	7. Create fields for any UI element that the cell needs to change.
	8. Update the fields in the Refresh() function.	
	
	9. Create a new script.
	10. Inherit from Menu<TCell, TData>.
	11. Add the component to a UI element.
	12. Create data for the menu to use.
	13. Set the data using the SetData(Tdata) function.
	14. Call Refresh() whenever the Menu needs to update.