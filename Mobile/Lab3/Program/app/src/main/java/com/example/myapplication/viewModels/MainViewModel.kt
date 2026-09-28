package com.example.myapplication.viewModels

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import com.example.myapplication.states.NodState

class MainViewModel : ViewModel()
{

    val currentState by mutableStateOf(NodState())

    fun onAction(numbers: MutableList<Int>)
    {
        currentState.numbers.clear()
        currentState.numbers.addAll(numbers);
        currentState.result = FindNod()
    }

    private  fun FindNod() : Int
    {
        for(num in currentState.numbers)
        {

        }
    }

    private  fun Calculate(a : Int, b : Int) : Int
    {
        var tmpA = a;
        var tmpB = b;
       while(tmpB != 0)
       {
        var temp = a % b;

           tmpA = tmpB;
           tmpB = temp;
       }
        return  
    }

}