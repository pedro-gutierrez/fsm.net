using System;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Key k = new Key( "a", 1 );
            Key k2 = new Key( "b", 2 );

            k.Tell( "login" );
            k.Tell( "login" );
            k.Tell( "login" );
            k.Tell( "login" );
            k.Tell( "login" );

        }


        class Key : Fsm.Of<int> 
        {
            private static string KEY = "key";

            public Key( string id, int start ) : base( KEY, id, start ) {


            }
            
            static Key()
            {
                Setup( KEY, fsm => {

                    fsm.In( "LOGGED_OUT" )
                            .When( "login" ).Then( "LOGGED_OUT" ).And( (args, data) => {
                                Console.WriteLine( "data: {0}", data );
                                return data+1;

                            }).WithTimeout( 1 );
               
                    fsm.In( "LOGGED_IN" );
                    

                    
                });
            }

        }


    }
}
