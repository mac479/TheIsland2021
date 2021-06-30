
public class Block{

	private int id;
	private bool air;
	private bool marker;

	public Block(int id){
		if(id == 0){
			air = true;
		}
		else{
			air = false;
		}
		this.id = id;
	}

	//Make the default constructor default to an air block.
	public Block(){
		id = 0;
		air = true;
		marker = false;
	}

	public int getID(){
		return id;
	}

	public bool isAir()
	{
		return air;
	}

	public void setMarker(bool val)
	{
		marker = val;
	}

	public bool getMarker()
	{
		return marker;
	}

}