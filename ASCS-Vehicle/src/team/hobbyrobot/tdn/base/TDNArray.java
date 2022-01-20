package team.hobbyrobot.tdn.base;

import java.util.Iterator;

import team.hobbyrobot.tdn.core.TDNTypeParser;
import team.hobbyrobot.tdn.core.TDNValue;

public class TDNArray implements Iterable<Object>
{
    public TDNArray(Object[] value, TDNTypeParser itemParser)
    {
        this.value = value;
        this.itemParser = itemParser;
    }

    public Object[] value;
    public TDNTypeParser itemParser;
    
    public Object get(int index)
    {
    	return value[index];
    }
    
    public void set(int index, Object value)
    {
    	this.value[index] = value;
    }

    public int size()
    {
    	return value.length;
    }
    
	@Override
	public Iterator<Object> iterator()
	{
		return new Iterator<Object>()
    	{
    		private int index = 0;

			@Override
			public boolean hasNext()
			{
				return index < value.length;
			}

			@Override
			public Object next()
			{
				return value[index++];
			}

			@Override
			public void remove()
			{
				throw new UnsupportedOperationException();
			}
    	};
	}
}