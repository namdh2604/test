#import <Foundation/Foundation.h>


NSString* strToNSStr(const char* str)
{
	if (!str)
		return [NSString stringWithUTF8String: ""];

	return [NSString stringWithUTF8String: str];
}


NSMutableDictionary* KeyValueToDict(const char* keys, const char* values)
{
    if (!keys || !values)
    {
        return nil;
    }
    
	NSMutableDictionary* dict = [[NSMutableDictionary alloc] init];

	NSArray* keysArray = [strToNSStr(keys) componentsSeparatedByString : @"\n"];
	NSArray* valuesArray = [strToNSStr(values) componentsSeparatedByString : @"\n"];

	for (int i = 0; i < [keysArray count]; i++)
	{
		[dict setObject:[valuesArray objectAtIndex: i] forKey:[keysArray objectAtIndex: i]];
	}
    
    NSLog(@"UnityFlurry::keyValueToDict >>> \n%@", dict);

	return dict;
}


int main(int argc, char *argv[]) 
{
	char* keys = "hello\nfoo";
	char* values = "world\nbar";
	KeyValueToDict(keys, values);
}

