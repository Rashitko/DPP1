#!/bin/bash
i='empty'
echo '' > input.txt
echo "testing with: $(cat input.txt)"
mono huffman.exe input.txt > original_output$i.txt
mono huffman_refaktor.exe input.txt > refactor_output$i.txt
diff original_output$i.txt refactor_output$i.txt > diff_output$i.txt
echo "diff:"
cat diff_output$i.txt

i='1'
echo '1' > input.txt
echo "testing with: $(cat input.txt)"
mono huffman.exe input.txt > original_output$i.txt
mono huffman_refaktor.exe input.txt > refactor_output$i.txt
diff original_output$i.txt refactor_output$i.txt > diff_output$i.txt
echo "diff:"
cat diff_output$i.txt

for i in $(seq -f "%02g" 1 10)
do
	input_string=$(date +%s%N|md5sum)
	echo $input_string > input.txt
	echo "testing with: $(cat input.txt)"
	mono huffman.exe input.txt > original_output$i.txt
	mono huffman_refaktor.exe input.txt > refactor_output$i.txt
	diff original_output$i.txt refactor_output$i.txt > diff_output$i.txt
	echo "diff:"
	cat diff_output$i.txt
done