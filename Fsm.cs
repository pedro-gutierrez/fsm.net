using System;
using System.Collections.Generic;
using System.Linq;

namespace Fsm
{
    abstract class Of<D>
    {
        
        private string Name;
        
        private string Id;

        private Fsm.Def<D> FsmDef; 

        private Def<D>.State FsmState;

        private D Data;

        public static void Setup( string name, Action<Fsm.Def<D>> setup ) {

            Fsm.Def<D> fsm = new Fsm.Def<D>();
            setup( fsm );
            Def.Register( name, fsm );

        }

        private void printMsg( string msg )
        {
             Console.WriteLine( "[{0}-{1}] [{2}] {3}", Name, Id, FsmState.Name, msg );
        }

        public Of( string name, string id, D data ) 
        {
            Name = name;
            Id = id;
            Data = data;
            FsmDef = ( Def<D> )Def.Find( name );
            FsmState = FsmDef.Initial;
            printMsg( "Init" );

        }

        public void Tell( string msg )
        {
            Tell( new Msg( msg ));
        }
        
        public void Tell( Msg msg )
        {
            List<Def<D>.Transition> ts = CollectionUtils.GetValue( FsmState.Transitions, msg.Name );
            if( ts != null ) {

                printMsg( "<- " + msg.Name );
                var t = ts.First();
                try {

                    Data = t.Action( msg.Args, Data );
                    if( FsmState.Name != t.Next ) 
                    {
                        printMsg( "-> [" + t.Next + "]" );
                    }

                    FsmState = CollectionUtils.GetValue( FsmDef.States, t.Next );

                } catch( Exception e ) {
                    printMsg( e.Message );
                }
                

            } else printMsg( msg.Name + " ?" );
            
           



        }
    } 


    public class Msg 
    {
        public readonly string Name;
        public readonly object Args;

        public Msg( string name )
        {
            Name = name;
        }

        public Msg( string name, object args )
        {
            Name = name;
            Args = args;
        }

    }

    public abstract class Def {

        public string Name;


        public static Dictionary<string, Def> Registry = new Dictionary<string, Def>();


        public static void Register( string name, Def def ) 
        {
            Registry.Add( name, def );
        }

        public static Def Find( string name ){

            return CollectionUtils.GetValue( Registry, name );
            
        }

    }

    public class Def<D> : Def
    {
        public State Initial;
        
        public readonly Dictionary<string, State> States 
            = new Dictionary<string, State>();

        public State In( string name )
        {
            var state = new State( name );
            States.Add( name, state );
            if( Initial == null ) Initial = state;
            return state;
        }
    
        public class Transition 
        {
            public string Next;
            public State State;
            public string Name;

            public int Timeout;

            public Func<object, D, D> Action;

            public Transition( State s, string name)
            {
                State = s;
                Name = name;
            }

        }

        public class State 
        {
            public string Name;
            private Transition Current;
            
            public readonly Dictionary<string, List<Transition>> Transitions =
                new Dictionary<string, List<Transition>>();

            public State( string name )
            {
                Name = name;
            }
            
            public State When( string msg )
            {
                var msgTransitions = CollectionUtils.GetValue( Transitions, msg );
                if( msgTransitions == null ) 
                {
                    msgTransitions = new List<Transition>();
                    Transitions.Add( msg, msgTransitions );
                }
                
                var t = new Transition( this, msg );
                msgTransitions.Add( t );
                Current = t;
                return this;
            }

            public State Then( string n )
            {
                Current.Next = n;
                return this;
            }


            public State And( Func<object, D, D> a ) 
            {
                Current.Action = a;
                return this;
            }

            public State WithTimeout( int secs )
            {
                Current.Timeout = secs;
                
                return this;
            }

            
        }




    
    }


    public static class CollectionUtils 
    {
        public static V GetValue<K, V>(this Dictionary<K, V> dic, K key)
        {
            V ret;
            bool found = dic.TryGetValue(key, out ret);
            if (found)
            {
                return ret;
            }
            return default(V);
        
        }
    }

}