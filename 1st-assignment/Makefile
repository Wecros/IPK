SHELL=bash
LOGIN=xfilip46

zip:
	zip $(LOGIN)-src.zip fileget.py socket_api.py README.md
	zip $(LOGIN)-out.zip output/*

clean:
	rm -rf $(LOGIN)-{src,out}.zip output/* 
	find . -type d -name __pycache__ | xargs rm -rf
