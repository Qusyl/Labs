package com.example.lab5true

import android.os.Bundle
import android.text.InputType
import android.widget.ArrayAdapter
import android.widget.AutoCompleteTextView
import android.widget.LinearLayout
import androidx.activity.viewModels
import androidx.appcompat.app.AppCompatActivity
import com.example.lab5true.databinding.ActivityMainBinding
import com.example.lab5true.viewModels.MainViewModel
import com.google.android.material.textfield.TextInputEditText
import com.google.android.material.textfield.TextInputLayout

class MainActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMainBinding
    private val viewModel: MainViewModel by viewModels()

    private val numberFields = mutableListOf<TextInputEditText>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        setupDropdown()
        binding.btnFind.setOnClickListener { viewModel.onFindClick() }

        viewModel.state.observe(this) { state ->
            renderNumbers(state.numbers)
            binding.txtResult.setText(state.result?.toString() ?: "")
            binding.txtError.text = state.error ?: ""
            binding.txtError.visibility =
                if (state.error.isNullOrEmpty()) android.view.View.GONE
                else android.view.View.VISIBLE
        }
    }

    private fun setupDropdown() {
        val options = listOf("2", "3", "4")
        val adapter = ArrayAdapter(this, android.R.layout.simple_list_item_1, options)
        binding.countDropdown.setAdapter(adapter)
        binding.countDropdown.setOnItemClickListener { _, _, position, _ ->
            viewModel.onCountChange(options[position].toInt())
        }

        binding.countDropdown.setText("2", false)
    }

    private fun renderNumbers(numbers: List<String>)
    {
        if (numberFields.size != numbers.size) {
            binding.numbersContainer.removeAllViews()
            numberFields.clear()

            numbers.forEachIndexed { index, value ->
                val textIntputLay = TextInputLayout(this).apply {
                    hint = "Число ${index + 1}"
                    layoutParams = LinearLayout.LayoutParams(
                        LinearLayout.LayoutParams.MATCH_PARENT,
                        LinearLayout.LayoutParams.WRAP_CONTENT
                    ).apply { topMargin = 8 }
                }
                val textInputEditedText = TextInputEditText(this).apply {
                    inputType = InputType.TYPE_CLASS_NUMBER
                    setText(value)
                    addTextChangedListener(SimpleTextWatcher { s ->
                        viewModel.onNumberChange(index, s)
                    })
                }
                textIntputLay.addView(textInputEditedText)
                binding.numbersContainer.addView(textIntputLay)
                numberFields.add(textInputEditedText)
            }
        } else {
            numbers.forEachIndexed { index, value ->
                val et = numberFields[index]
                if (et.text.toString() != value) et.setText(value)
            }
        }
    }
}
class SimpleTextWatcher(private val onText: (String) -> Unit) : android.text.TextWatcher {
    override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {}
    override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {
        onText(s?.toString() ?: "")
    }
    override fun afterTextChanged(s: android.text.Editable?) {}
}