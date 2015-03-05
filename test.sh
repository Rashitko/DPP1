#!/bin/bash
for i in $(seq -f "%02g" 1 10)
do
	input_string=$(date +%s%N|md5sum)
	echo $input_string > input.txt
	echo "testing with: $input_string"
	mono huffman.exe input.txt > original_output$i.txt
	mono huffman_refaktor.exe input.txt > refactor_output$i.txt
	diff original_output$i.txt refactor_output$i.txt > diff_output$i.txt
	echo "result:"
	cat diff_output$i.txt
done
