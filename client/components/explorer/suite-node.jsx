/** @jsx React.DOM */
'use strict';

var React = require('react');
var SpecLeaf = require('./spec-leaf');

var icons = require('./../icons');

var CommandLink = require('./command-link');

var FolderOpen = icons['folder-open'];
var FolderClosed = icons['folder-closed'];
var CommandWithNameEntryLink = require('./command-with-name-entry-link');

var SuiteHeader = React.createClass({
	getInitialState: function () {
		return {
			open: true
		};
	},
	makeNewSuiteLink: function(){
		var self = this;

		var toMessage = name => {
			return {
				type: 'add-suite',
				name: name,
				parent: self.props.suite.path
			};
		};

		var title = 'Add a new Child Suite to ' + self.props.suite.path;

		return (
			<CommandWithNameEntryLink
				title={title}
				text='new child suite'
				commandText='Create'
				toMessage={toMessage}/>
		);
	},

	makeNewSpecLink: function(){
		var self = this;

		var toMessage = function(name){
			return {
				type: 'add-spec',
				name: name,
				parent: self.props.suite.path
			};
		};

		var title = 'Add a new Specification to ' + self.props.suite.path;

		return (
			<CommandWithNameEntryLink
				title={title}
				text='new spec'
				commandText='Create'
				toMessage={toMessage}/>
		);
	},

	openFolder: function () {
		this.setState({ open: !this.state.open });
	},

	render: function(){
		var suite = this.props.suite;

		var buildMessage = function(){
			var list = suite.allSpecs().map(spec => spec.id);

			return {type: 'run-specs', list: list};
		};

		var href = '#/suite/' + suite.path;
		var openClosed = this.state.open ? <FolderOpen /> : <FolderClosed />;
		var openClass = this.state.open ? 'open' : 'closed';

		return (
			<div className={openClass + ' suite-header'}>
				<a href='#' onClick={this.openFolder}>
					{openClosed}
				</a>
				<a href={href} className='suite-name'>{this.props.suite.name}</a>
				<CommandLink createMessage={buildMessage} text='run all' />
				{this.makeNewSpecLink()}
				{this.makeNewSuiteLink()}
			</div>

		);
	}
});

var SuiteBody = React.createClass({
	getInitialState: function () {
		return {
			maxHeight: '0px'
		};
	},
	render: function(){
		var childSuites = this.props.suite.suites.map(suite => (<SuiteNode suite={suite} key={suite.path} />) );
		var specs = this.props.suite.specs.map(spec => (<SpecLeaf spec={spec} key={spec.id} />) );
		var style = {
			maxHeight: this.state.maxHeight
		};

		return (
			<div id={this.props.suite.path} className='suite-body' ref='suiteBody' style={style}>
				{childSuites}
				{specs}
			</div>
		);
	},
	componentDidMount: function () {
		this.setState({maxHeight: this.refs.suiteBody.getDOMNode().scrollHeight + 'px'});
	}
});

var SuiteNode = React.createClass({
	render: function(){
		return (
			<div className='suite-node'>
				<SuiteHeader suite={this.props.suite} />
				<SuiteBody suite={this.props.suite} />
			</div>
		);
	}
});

module.exports = SuiteNode;
