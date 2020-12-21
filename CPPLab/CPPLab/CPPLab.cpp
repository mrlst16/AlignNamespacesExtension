// CPPLab.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <list>
#include <string>
#include <vector>
#include <algorithm>

using namespace std;


ostream& operator<< (ostream& out, const list<string>& lst) {
	for (auto it = lst.begin(); it != lst.end(); it++) {
		string letter = *it;
		out << letter << endl;
	}
	out << "STOP" << endl;
	return out;
}
//
//ostream& operator<< (ostream& out, const list<size_t>& lst) {
//	for (auto it = lst.begin(); it != lst.end(); it++) {
//		int i = *it;
//		out << i << endl;
//	}
//	out << "STOP" << endl;
//	return out;
//}


int IntSize(string d) {
	return d.size();
}

std::list<size_t> getLengths(const std::vector<std::string>& v0) {
	std::list<size_t> result(v0.size());
	std::transform(v0.cbegin(), v0.cend(), result.begin(), [](string s) -> int {return s.size();});
	return result;
};

bool uniformlyLessThan(
	const list<int>& L1,
	const list<int>& L2) {

	if (L1.size() != L2.size()) return false;

	auto it1 = L1.begin();
	auto it2 = L2.begin();
	for (; it1 != L1.end(); it1++, it2++) {
		int one = (*it1);
		int two = (*it2);

		if (one >= two) return false;
	}
	return true;
}


int main()
{
	vector<string> testData;
	testData.push_back("one");
	testData.push_back("two");
	testData.push_back("three");

	auto result = getLengths(testData);

}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
