<template>
	<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="成绩管理" sub-title="成绩信息基本操作" />
	<a-form :model="formState" name="horizontal_query" layout="inline" autocomplete="off" @finish="onFinish" @finishFailed="onFinishFailed">
		<a-form-item label="学生名称" name="name">
			<a-input v-model:value="formState.studentName"></a-input>
		</a-form-item>
		<a-form-item label="课程名称" name="no">
			<a-input v-model:value="formState.courseName"></a-input>
		</a-form-item>
		<a-form-item>
			<a-button type="primary" html-type="submit">查询</a-button>
		</a-form-item>
		<a-form-item>
			<a-button type="primary" @click="add">新增</a-button>
		</a-form-item>
	</a-form>
	<a-table :columns="columns" :data-source="dataSource" bordered :pagination="false" :row-key="record => record.id">
		<template #bodyCell="{ column, text, record }">
			<template v-if="[ 'name','teacher', 'createUser','lastEditUser'].includes(column.dataIndex)">
				<div>{{ text }}</div>
			</template>
			<template v-else-if="[ 'createTime','lastEditTime'].includes(column.dataIndex)">
				<div>{{formatDateString(text)}}</div>
			</template>
			<template v-else-if="column.dataIndex === 'operation'">
				<div class="editable-row-operations">
					<a @click="edit(record.id)">Edit</a>
				</div>
			</template>
		</template>
	</a-table>
	<a-pagination v-model:current="pagination.current" :total="pagination.total" @change="change" />
	<a-modal ref="modalRef" v-model:visible="visible" okText="保存" cancelText="取消" :wrap-style="{ overflow: 'hidden' }" @ok="handleOk">
		<div>
			<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="成绩管理" sub-title="新增或编辑成绩" />
			<br />
			<a-form :model="addEditFormState">
				<a-form-item label="学生名称">
					<a-select ref="select" v-model:value="addEditFormState.studentId" style="width: 200px">
						<a-select-option :value="item.id" v-for="(item) in dataStudents" :key="item.id">{{item.name}}</a-select-option>
					</a-select>
				</a-form-item>
				<a-form-item label="课程名称">
					<a-select ref="select" v-model:value="addEditFormState.courseId" style="width: 200px">
						<a-select-option :value="item.id" v-for="(item) in dataCourses" :key="item.id">{{item.name}}</a-select-option>
					</a-select>
				</a-form-item>
				<a-form-item label="考试成绩">
					<a-input v-model:value="addEditFormState.score" />
				</a-form-item>
			</a-form>
		</div>
	</a-modal>
</template>
<script lang="ts">
	import {
		defineComponent,
		reactive,
		toRefs,
		ref,
		toRaw
	} from 'vue';
	import type {
		UnwrapRef
	} from 'vue';
	import {
		message
	} from 'ant-design-vue';
	import {
		getD,
		postD
	} from '../api/index.js';

	const columns = [{
			title: '课程名称',
			dataIndex: 'courseName',
			key: 'courseName',
			align: 'center',
			width: '10%',
		},
		{
			title: '学生名称',
			dataIndex: 'studentName',
			key: 'studentName',
			align: 'center',
			width: '10%',
		},
		{
			title: '考试成绩',
			dataIndex: 'score',
			key: 'score',
			align: 'center',
			width: '10%',
		},
		{
			title: '创建人员',
			dataIndex: 'createUser',
			key: 'createUser',
			align: 'center',
			width: '10%',
		},
		{
			title: '创建时间',
			dataIndex: 'createTime',
			key: 'createTime',
			align: 'center',
			width: '10%',
		},
		{
			title: '编辑人员',
			dataIndex: 'lastEditUser',
			key: 'lastEditUser',
			align: 'center',
			width: '10%',
		},
		{
			title: '编辑时间',
			dataIndex: 'lastEditTime',
			key: 'lastEditTime',
			align: 'center',
			width: '10%',
		},
		{
			title: '操作',
			dataIndex: 'operation',
			key: 'operation',
			align: 'center',
			width: '10%',
		},
	];
	interface DataItem {
		id: number,
		studentId: string,
		studentName: string,
		courseId: string,
		courseName: string,
		score: string,
		createTime: Date,
		createUser: string,
		lastEditTime: Date,
		lastEditUser: string
	}
	interface FormState {
		studentName: string;
		courseName: string;
	}
	interface StudentItem {
		id: number,
		name: string
	}
	interface CourseItem {
		id: number,
		name: string
	}
	const pagination = {
		total: 1,
		current: 1,
		pageSize: 10,
	};
	const dataSource: DataItem[] = [];
	const dataStudents: StudentItem[] = [];
	const dataCourses: CourseItem[] = [];
	export default defineComponent({
		setup() {
			const formatDateString = (value) => {
				var dt = new Date(value);
				let year = dt.getFullYear();
				let month = (dt.getMonth() + 1).toString().padStart(2, '0');
				let date = dt.getDate().toString().padStart(2, '0');
				let hour = dt.getHours().toString().padStart(2, '0');
				let minute = dt.getMinutes().toString().padStart(2, '0');
				let second = dt.getSeconds().toString().padStart(2, '0');
				return `${year}-${month}-${date} ${hour}:${minute}:${second}`;
			};
			const formState = reactive < FormState > ({
				studentName: '',
				courseName: '',
			});
			const addEditFormState = reactive({
				id: 0,
				studentId: '',
				courseId: '',
				score: ''
			});
			const visible = ref < boolean > (false);
			const add = () => {
				addEditFormState.id = -1;
				addEditFormState.studentId = '';
				addEditFormState.courseId = '';
				addEditFormState.score = '';
				visible.value = true;
			};

			const handleOk = (e: MouseEvent) => {
				console.log(e);
				console.log(addEditFormState);
				var url = "";
				if (addEditFormState.id > 0) {
					url = "/api/Score/UpdateScore"; //编辑
				} else {
					url = "/api/Score/AddScore"; //新增
				}
				postD(url, {
					"id": addEditFormState.id > 0 ? addEditFormState.id : null,
					"studentId": addEditFormState.studentId,
					"courseId": addEditFormState.courseId,
					"score": addEditFormState.score,
					"createTime": new Date(),
					"createUser": 0,
					"lastEditTime": new Date(),
					"lastEditUser": 0
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						if (res.data == 0) {
							message.success('保存成功！');
							visible.value = false;
							var studentName = formState.studentName;
							var courseName = formState.courseName;
							getScores(studentName, courseName);
						} else {
							message.error('保存失败！');
						}
					}
				});

			};
			const onFinish = (values: any) => {
				var studentName = formState.studentName;
				var courseName = formState.courseName;
				getScores(studentName, courseName);
				console.log('Success:', values);
			};

			const onFinishFailed = (errorInfo: any) => {
				console.log('Failed:', errorInfo);
			};
			const state = reactive({
				dataSource: dataSource,
				dataStudents: dataStudents,
				dataCourses: dataCourses
			});
			const getStudents = () => {
				getD('/api/Student/GetStudents', {
					"no": "",
					"name": "",
					"pageSize": 0,
					"pageNum": 0
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						for (let i = 0; i < res.data.items.length; i++) {
							dataStudents.push({
								id: res.data.items[i].id,
								name: res.data.items[i].name,
							});
						}
						state.dataStudents = [...dataStudents];
					}
				});
			};
			getStudents();
			const getCourses = () => {
				getD('/api/Course/GetCourses', {
					"courseName": "",
					"teacher": "",
					"pageSize": 0,
					"pageNum": 0
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						for (let i = 0; i < res.data.items.length; i++) {
							dataCourses.push({
								id: res.data.items[i].id,
								name: res.data.items[i].name,
							});
						}
						state.dataCourses = [...dataCourses];
					}
				});
			};
			getCourses();
			const getScores = (studentName, courseName) => {
				dataSource.length = 0;
				getD('/api/Score/GetScores', {
					"pageSize": pagination.pageSize,
					"pageNum": pagination.current,
					"studentName": studentName,
					"courseName": courseName
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						pagination.total = res.data.count; //总记录数
						console.log(pagination.total);
						for (let i = 0; i < res.data.items.length; i++) {
							dataSource.push({
								id: res.data.items[i].id,
								studentName: res.data.items[i].studentName,
								studentId: res.data.items[i].studentId,
								courseName: res.data.items[i].courseName,
								courseId: res.data.items[i].courseId,
								score: res.data.items[i].score,
								"createTime": res.data.items[i].createTime,
								"createUser": res.data.items[i].createUser,
								"lastEditTime": res.data.items[i].lastEditTime,
								"lastEditUser": res.data.items[i].lastEditUser,
							});
						}
						state.dataSource = [...dataSource];
					}
				});
			};
			getScores(null, null);
			const editableData: UnwrapRef < Record < string, DataItem >> = reactive({});

			const edit = (id: number) => {
				console.log(id);
				var score = dataSource.filter(item => id === item.id)[0];
				addEditFormState.id = score.id;
				addEditFormState.studentId = score.studentId;
				addEditFormState.courseId = score.courseId;
				addEditFormState.score=score.score;
				visible.value = true;
				console.log(score);
			};

			const change = (pagination) => {
				var studentName = formState.studentName;
				var courseName = formState.courseName;
				getScores(studentName, courseName);
				console.log(pagination);
			};
			const onSubmit = () => {
				console.log('submit!', toRaw(formState));
			};
			return {
				formatDateString,
				formState,
				addEditFormState,
				...toRefs(state),
				columns,
				editingKey: '',
				editableData,
				edit,
				pagination,
				change,
				onFinish,
				onFinishFailed,
				onSubmit,
				visible,
				add,
				handleOk,
			};
		},
	});
</script>
<style scoped>
	.editable-row-operations a {
		margin-right: 8px;
	}

	.ant-form {
		height: 6vh;
		width: 100vh;
		background-color: transparent;
	}

	.ant-modal-content {
		height: 50vh;
	}

	.ant-modal-body .ant-input {
		width: 40vh;
	}

	.ant-modal-body {
		height: 40vh;
	}

	.ant-modal-body .ant-page-header {
		width: 46vh;
		padding: 0.5rem;
	}
</style>
