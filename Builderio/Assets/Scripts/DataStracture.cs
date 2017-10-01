using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node<T>
{
    private T value;                           // Node value
    private Node<T> next;               // next Node

    /* Constructor  - returns a Node with "value" as value and without successesor Node **/
    public Node(T value)
    {
        this.value = value;
        this.next = null;
    }

    /* Constructor  - returns a Node with "value" as value and its successesor is "next" **/
    public Node(T value, Node<T> next)
    {
        this.value = value;
        this.next = next;
    }

    /* Returns the Node "value" **/
    public T GetValue()
    {
        return this.value;
    }

    /* Returns the Node "next" Node **/
    public Node<T> GetNext()
    {
        return this.next;
    }

    /* Return if the current Node Has successor **/
    public bool HasNext()
    {
        return (this.next != null);
    }

    /* Set the value attribute to be "value" **/
    public void SetValue(T value)
    {
        this.value = value;
    }

    /* Set the next attribute to be "next" **/
    public void SetNext(Node<T> next)
    {
        this.next = next;
    }

    /* Returns a string that describes  the Node (and its' successesors **/
    public override string ToString()
    {
        if (next == null)
            return value.ToString() + " --> null";
        return value.ToString() + " --> " + next;
    }
}

public class Queue<T>
{
    public Node<T> first;
    public Node<T> last;


    public Queue()
    {
        this.first = null;
        this.last = null;
    }

    public void Insert(T x)
    {
        Node<T> temp = new Node<T>(x);
        if (first == null)
            first = temp;
        else
            last.SetNext(temp);
        last = temp;
    }

    public T Remove()
    {
        T x = first.GetValue();
        first = first.GetNext();
        if (first == null)
            last = null;
        return x;
    }

    public T Top()
    {
        return first.GetValue();
    }

    public bool IsEmpty()
    {
        return first == null;
    }

    public override string ToString()
    {
        Node<T> current = first;
        string temp = "";
        while (current != null)
        {
            temp += current.ToString() + " --> ";
            current = current.GetNext();
        }
        return "head -> " + temp + "end";
    }
}
